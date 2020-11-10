using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class WorkflowEngine : IWorkflowEngine
	{
		IInstanceStorage _instanceStorage;
		
		public WorkflowEngine(IInstanceStorage instanceStorage)
		{
			_instanceStorage = instanceStorage ?? throw new ArgumentNullException(nameof(instanceStorage));
		}

		public async ValueTask<IInstance> StartAsync(IActivity root, Object args = null)
		{
			var inst = new Instance()
			{
				Id = Guid.NewGuid(),
				Root = root
			};
			root.OnEndInit();
			var context = new ExecutionContext(inst.Root, args);
			context.Schedule(inst.Root, null);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _instanceStorage.Save(inst);
			return inst;
		}

		public async ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null)
		{
			var inst = await _instanceStorage.Load(id);
			inst.Root.OnEndInit();
			var context = new ExecutionContext(inst.Root);
			context.SetState(inst.State);
			await context.ResumeAsync(bookmark, reply);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _instanceStorage.Save(inst);
			return inst;
		}
	}
}
