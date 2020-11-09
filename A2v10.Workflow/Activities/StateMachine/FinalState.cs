
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;
using Microsoft.VisualBasic;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class FinalState : State
	{
		public override Boolean IsFinal => false;

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			if (onComplete != null)
				return onComplete.Invoke(context, this);
			return new ValueTask();
		}
	}
}
