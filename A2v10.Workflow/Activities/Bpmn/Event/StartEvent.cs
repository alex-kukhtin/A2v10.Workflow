﻿
using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StartEvent : Event
	{
		public override Boolean IsStart => true;

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			context.Execute(Id, nameof(Script));

			if (Children == null)
				return onComplete(context, this);

			foreach (var flow in Outgoing)
			{
				var flowElem = Parent.FindElement<SequenceFlow>(flow.Text);
				if (flowElem.SourceRef != Id)
					throw new WorkflowException($"BPMN. Invalid SequenceFlow (Id={Id}. SourceRef does not match");
				// generate new token for every outogoing flow!
				context.Schedule(flowElem, onComplete, Parent.NewToken());
			}
			return ValueTask.CompletedTask;
		}
	}
}
