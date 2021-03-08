using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class EndEvent : Event
	{
		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			context.Execute(Id, nameof(Script));

			Parent.KillToken(token);
			if (onComplete != null)
				return onComplete(context, this);
			return ValueTask.CompletedTask;
		}
	}
}
