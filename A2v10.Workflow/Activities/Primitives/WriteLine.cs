using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class WriteLine : Activity, IScriptable
	{
		public String Expression { get; set; }

		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecute(nameof(Expression), Expression);
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			var val = context.Evaluate<Object>(Ref, nameof(Expression));
			Console.WriteLine(val);
			return onComplete(context, this);
		}
	}
}
