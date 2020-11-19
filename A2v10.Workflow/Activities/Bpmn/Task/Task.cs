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
		public List<String> Outgouing { get; init; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			if (Outgouing == null)
				return onComplete(context, this);
			if (Outgouing.Count == 1)
			{
				// simple outgouning - same token
				var targetFlow = Parent.FindElement<SequenceFlow>(Outgouing[0]);
				context.Schedule(targetFlow, onComplete, token);
			}
			else
			{
				// same as Task + parallelGateway
				foreach (var flowId in Outgouing)
				{
					Parent.KillToken(token);
					var targetFlow = Parent.FindElement<SequenceFlow>(flowId);
					context.Schedule(targetFlow, onComplete, Parent.NewToken());
				}
			}
			return new ValueTask();
		}
	}
}
