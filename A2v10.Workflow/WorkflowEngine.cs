
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Interfaces.Api;

namespace A2v10.Workflow
{
	public class WorkflowEngine : IWorkflowEngine, IWorkflowApi
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IWorkflowStorage _workflowStorage;
		private readonly IInstanceStorage _instanceStorage;
		private readonly ITracker _tracker;

		public WorkflowEngine(IServiceProvider serviceProvider, ITracker tracker)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_workflowStorage = _serviceProvider.GetService<IWorkflowStorage>() ?? throw new NullReferenceException("IWorkflowStorage");
			_instanceStorage = _serviceProvider.GetService<IInstanceStorage>() ?? throw new NullReferenceException("IInstanceStorage");
			_tracker = tracker;
		}

		public async ValueTask<IInstance> CreateAsync(IActivity root, IIdentity identity)
		{
			var inst = new Instance()
			{
				Id = Guid.NewGuid(),
				Workflow = new Workflow() {
					Root = root,
					Identity = identity
				}
			};
			root.OnEndInit();
			await _instanceStorage.Create(inst);
			return inst;
		}

		public async ValueTask<IInstance> CreateAsync(IIdentity identity)
		{
			var wf = await _workflowStorage.LoadAsync(identity);
			return await CreateAsync(wf.Root, wf.Identity);
		}

		public async ValueTask<IInstance> RunAsync(IInstance instance, Object args = null)
		{
			if (instance.ExecutionStatus != WorkflowExecutionStatus.Init)
				throw new WorkflowExecption($"Instance (id={instance.Id}) is already running");
			var context = new ExecutionContext(_serviceProvider, _tracker, instance.Workflow.Root, args);
			context.Schedule(instance.Workflow.Root, null, null);
			await context.RunAsync();
			SetInstanceState(instance, context);
			await _instanceStorage.Save(instance);
			return instance;
		}

		public async ValueTask<IInstance> RunAsync(Guid id, Object args = null)
		{
			IInstance instance = await _instanceStorage.Load(id);
			return await RunAsync(instance, args);
		}


		public async ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null)
		{
			var inst = await _instanceStorage.Load(id);
			inst.Workflow.Root.OnEndInit();
			var context = new ExecutionContext(_serviceProvider, _tracker, inst.Workflow.Root);
			context.SetState(inst.State);
			await context.ResumeAsync(bookmark, reply);
			await context.RunAsync();
			SetInstanceState(inst, context);
			await _instanceStorage.Save(inst);
			return inst;
		}

		static void SetInstanceState(IInstance inst, ExecutionContext context)
		{
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			inst.ExecutionStatus = context.GetExecutionStatus();
			var instData = new InstanceData()
			{
				ExternalVariables = context.GetExternalVariables(inst.State),
				ExternalBookmarks = context.GetExternalBookmarks(),
				TrackRecords = context.GetTrackRecords()
			};
			inst.InstanceData = instData;
		}

		public async ValueTask<IResumeProcessResponse> ResumeAsync(IResumeProcessRequest prm)
		{
			await ResumeAsync(prm.InstanceId, prm.Bookmark, prm.Result);
			return new ResumeProcessResponse();
		}


		public async ValueTask<IStartProcessResponse> StartAsync(IStartProcessRequest prm)
		{
			var i = await CreateAsync(prm.Identity);
			i = await RunAsync(i.Id, prm.Parameters);
			return new StartProcessResponse()
			{
				InstanceId = i.Id
			};
		}

		public async ValueTask<ICreateProcessResponse> CreateAsync(ICreateProcessRequest prm)
		{
			var i = await CreateAsync(prm.Identity);
			return new StartProcessResponse()
			{
				InstanceId = i.Id
			};
		}

		public async ValueTask<IRunProcessResponse> RunAsync(IRunProcessRequest prm)
		{
			await RunAsync(prm.InstanceId, prm.Parameters);
			return new ResumeProcessResponse();
		}
	}
}
