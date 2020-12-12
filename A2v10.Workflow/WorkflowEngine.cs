
using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class WorkflowEngine : IWorkflowEngine
	{
		private readonly IRepository _repository;
		private readonly ITracker _tracker;

		public WorkflowEngine(IRepository repository, ITracker tracker)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
		}

		public async ValueTask<IInstance> StartAsync(IActivity root, Object args = null)
		{
			var inst = new Instance()
			{
				Id = Guid.NewGuid(),
				Workflow = new Workflow() { Root = root}
			};
			root.OnEndInit();
			var context = new ExecutionContext(_tracker, inst.Workflow.Root, args);
			context.Schedule(inst.Workflow.Root, null, null);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _repository.InstanceStorage.Save(inst);
			return inst;
		}

		public async ValueTask<IInstance> StartAsync(IIdentity identity, Object args = null)
		{
			var wf = await _repository.WorkflowStorage.LoadAsync(identity);
			return await StartAsync(wf.Root, args);
		}


		public async ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null)
		{
			var inst = await _repository.InstanceStorage.Load(id);
			inst.Workflow.Root.OnEndInit();
			var context = new ExecutionContext(_tracker, inst.Workflow.Root);
			context.SetState(inst.State);
			await context.ResumeAsync(bookmark, reply);
			await context.RunAsync();
			inst.Result = context.GetResult();
			inst.State = context.GetState();
			await _repository.InstanceStorage.Save(inst);
			return inst;
		}
	}
}
