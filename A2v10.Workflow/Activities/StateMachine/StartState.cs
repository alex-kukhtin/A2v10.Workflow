
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;
using Microsoft.VisualBasic;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StartState : StateBase
	{
		public override Boolean IsStart => true;

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			NextState = Next;
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}
	}
}
