using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BpmnTask : BpmnElement
	{
		public List<String> Incoming { get; init; }
		public List<String> Outgoing { get; init; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			// boundary events
			foreach (var ev in Parent.FindAll<BoundaryEvent>(ev => ev.AttachedToRef == Id))
			{

			}

			if (Outgoing == null)
				return onComplete(context, this);

			if (Outgoing.Count == 1)
			{
				// simple outgouning - same token
				var targetFlow = Parent.FindElement<SequenceFlow>(Outgoing[0]);
				context.Schedule(targetFlow, onComplete, token);
			}
			else
			{
				// same as task + parallelGateway
				Parent.KillToken(token);
				foreach (var flowId in Outgoing)
				{
					var targetFlow = Parent.FindElement<SequenceFlow>(flowId);
					context.Schedule(targetFlow, onComplete, Parent.NewToken());
				}
			}
			return new ValueTask();
		}
	}
}
