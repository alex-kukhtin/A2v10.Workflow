using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public interface IActivity
	{
		String Id { get; }

		ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete);
		ValueTask CancelAsync(IExecutionContext context);

		IEnumerable<IActivity> EnumChildren();

		void OnEndInit();
	}
}

