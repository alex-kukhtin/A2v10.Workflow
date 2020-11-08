
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public interface IExecutionContext
	{
		void Schedule(IActivity activity, ExecutingAction onComplete);

		T Evaluate<T>(String expression);
		void Execute(String expression);
	}
}
