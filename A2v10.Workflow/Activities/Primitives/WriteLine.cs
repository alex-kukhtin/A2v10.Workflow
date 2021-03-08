using A2v10.Workflow.Interfaces;
using System;
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

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var val = context.Evaluate<Object>(Id, nameof(Expression));
			Console.WriteLine(val);
			if (onComplete != null)
				return onComplete(context, this);
			return ValueTask.CompletedTask;
		}
	}
}
