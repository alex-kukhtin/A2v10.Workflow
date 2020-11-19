
using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StartState : StateBase
	{
		public override Boolean IsStart => true;

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			NextState = Next;
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}
	}
}
