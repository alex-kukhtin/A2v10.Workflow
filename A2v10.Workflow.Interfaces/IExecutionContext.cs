
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;
	using ResumeAction = Func<IExecutionContext, String, Object, ValueTask>;

	public interface IExecutionContext
	{
		void Schedule(IActivity activity, ExecutingAction onComplete, IToken token);
		void SetBookmark(String bookmark, ResumeAction onComplete);
		void RemoveBookmark(String bookmark);

		T Evaluate<T>(String refer, String name);
		void Execute(String refer, String name);
		void ExecuteResult(String refer, String name, Object result);
	}
}
