
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public interface IActivityStorage
	{
		Boolean IsLoading { get; }
		Boolean IsStoring { get; }

		void Set<T>(String name, T value);
		void SetCallback(String name, ExecutingAction callback);

		ExecutingAction GetCallback(String name);
		T Get<T>(String name);
	}
}
