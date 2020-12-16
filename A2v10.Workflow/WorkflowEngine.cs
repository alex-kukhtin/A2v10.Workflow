
using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Interfaces.Api;

namespace A2v10.Workflow
{
	public class WorkflowEngine : IWorkflowEngine, IWorkflowApi
	{
		private readonly IWorkflowStorage _workflowStorage;
		private readonly IInstanceStorage _instanceStorage;
		private readonly ITracker _tracker;

		public WorkflowEngine(IWorkflowStorage workflowStorage, IInstanceStorage instanceStorage, ITracker tracker)
		{
			_workflowStorage = workflowStorage ?? throw new ArgumentNullException(nameof(workflowStorage));
			_instanceStorage = instanceStorage ?? throw new ArgumentNullException(nameof(instanceStorage));
			_tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
		}

		public async ValueTask<IInstance> StartAsync(IActivity root, IIdentity identity, Object args = null)
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
			var context = new ExecutionContext(_tracker, inst.Workflow.Root, args);
			context.Schedule(inst.Workflow.Root, null, null);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _instanceStorage.Save(inst);
			return inst;
		}

		public async ValueTask<IInstance> StartAsync(IIdentity identity, Object args = null)
		{
			var wf = await _workflowStorage.LoadAsync(identity);
			return await StartAsync(wf.Root, identity, args);
		}


		public async ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null)
		{
			var inst = await _instanceStorage.Load(id);
			inst.Workflow.Root.OnEndInit();
			var context = new ExecutionContext(_tracker, inst.Workflow.Root);
			context.SetState(inst.State);
			await context.ResumeAsync(bookmark, reply);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _instanceStorage.Save(inst);
			return inst;
		}

		public async ValueTask<IStartProcessResponse> StartAsync(IStartProcessRequest prm)
		{
			var i = await StartAsync(prm.Identity, prm.Parameters);
			return new StartProcessResponse()
			{
				InstanceId = i.Id
			};
		}

		public async ValueTask<IResumeProcessResponse> ResumeAsync(IResumeProcessRequest prm)
		{
			await ResumeAsync(prm.InstanceId, prm.Bookmark, prm.Result);
			return new ResumeProcessResponse();
		}
	}
}
