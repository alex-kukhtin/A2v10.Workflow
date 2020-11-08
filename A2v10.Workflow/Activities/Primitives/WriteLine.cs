using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class WriteLine : Activity
	{
		public String Expression { get; set; }

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			var val = context.Evaluate<Object>(Expression);
			Console.WriteLine(val);
			return onComplete(context, this);
		}
	}
}
