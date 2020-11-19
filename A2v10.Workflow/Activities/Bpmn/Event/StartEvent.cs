using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StartEvent : Event
	{
		public override Boolean IsStart => true;

		public List<String> Outgoing { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			if (Outgoing == null)
				return onComplete(context, this);
			foreach (var flow in Outgoing)
			{
				var flowElem = Parent.FindElement<SequenceFlow>(flow);
				if (flowElem.SourceRef != Id)
					throw new WorkflowExecption($"BPMN. Invalid SequenceFlow (Id={Id}. SourceRef does not match");
				// generate new token for every outogoing flow!
				context.Schedule(flowElem, onComplete, Parent.NewToken());
			}
			return new ValueTask();
		}
	}
}
