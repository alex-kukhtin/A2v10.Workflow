using System;
using System.Linq;

using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BoundaryEvent : Event
	{
		public String AttachedToRef { get; init; }

		public Boolean CancelActivity { get; init; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			foreach (var ev in Children.OfType<EventDefinition>())
			{
				//ev.Run();
			}
			return new ValueTask();
		}

		public async ValueTask OnTrigger(IExecutionContext context)
		{
			if (CancelActivity)
			{
				var task = Parent.FindElement<BpmnTask>(AttachedToRef);
				await task.CancelAsync(context);
			}
		}
	}
}
