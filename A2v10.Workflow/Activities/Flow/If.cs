
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class If : Activity, IScriptable
	{
		public String Condition { get; set; }

		public IActivity Then { get; set; }
		public IActivity Else { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Id, nameof(Condition));
			if (cond)
			{
				if (Then != null)
					context.Schedule(Then, onComplete, token);
				else if (onComplete != null)
					return onComplete(context, this);
			}
			else
			{
				if (Else != null)
					context.Schedule(Else, onComplete, token);
				else if (onComplete != null)
					return onComplete(context, this);
			}
			return new ValueTask();
		}

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Then != null)
				yield return Then;
			if (Else != null)
				yield return Else;
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildEvaluate(nameof(Condition), Condition);
		}
		#endregion
	}
}
