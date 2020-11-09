
using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

public class TraverseArg
{
	public Action<IActivity> Start;
	public Action<IActivity> Action;
	public Action<IActivity> End;
}

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public interface IActivity
	{
		String Ref { get; }

		ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete);
		ValueTask CancelAsync(IExecutionContext context);

		ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction);
		void Traverse(TraverseArg traverse);
	}
}

