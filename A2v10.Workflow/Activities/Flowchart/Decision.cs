
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Decision : Activity
	{
		public String Condition { get; set; }
		public String Then { get; set; }
		public String Else { get; set; }

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Condition);
			throw new NotImplementedException();
		}
	}
}
