using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Flowchart : Activity
	{
		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			throw new NotImplementedException();
		}
	}
}
