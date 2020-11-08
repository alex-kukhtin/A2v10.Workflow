using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public interface IActivity
	{
		String Ref { get; }

		ValueTask Execute(IExecutionContext context, ExecutingAction onComplete);
		ValueTask Cancel(IExecutionContext context);

		ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction);
		void Traverse(Action<IActivity> onAction);
	}
}
