﻿
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

		public WorkflowEngine(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_workflowStorage = _serviceProvider.GetService<IWorkflowStorage>() ?? throw new NullReferenceException("IWorkflowStorage");
			_instanceStorage = _serviceProvider.GetService<IInstanceStorage>() ?? throw new NullReferenceException("IInstanceStorage");
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
			var context = new ExecutionContext(_serviceProvider, inst.Workflow.Root, args);
			context.Schedule(inst.Workflow.Root, null, null);
			await context.RunAsync();
			SetInstanceState(inst, context);
			await _instanceStorage.Save(inst);
			return inst;
		}

		public async ValueTask<IInstance> StartAsync(IIdentity identity, Object args = null)
		{
			var wf = await _workflowStorage.LoadAsync(identity);
			return await StartAsync(wf.Root, wf.Identity, args);
		}


		public async ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null)
		{
			var inst = await _instanceStorage.Load(id);
			inst.Workflow.Root.OnEndInit();
			var context = new ExecutionContext(_serviceProvider, inst.Workflow.Root);
			context.SetState(inst.State);
			await context.ResumeAsync(bookmark, reply);
			await context.RunAsync();
			SetInstanceState(inst, context);
			await _instanceStorage.Save(inst);
			return inst;
		}

		void SetInstanceState(IInstance inst, ExecutionContext context)
		{
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			inst.ExternalVariables = context.GetExternalVariables(inst.State);
			inst.ExternalBookmarks = context.GetExternalBookmarks();
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
