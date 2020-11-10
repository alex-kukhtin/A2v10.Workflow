
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class If : Activity, IScriptable
	{
		public String Condition { get; set; }

		public IActivity Then { get; set; }
		public IActivity Else { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Ref, nameof(Condition));
			if (cond)
			{
				if (Then != null)
					context.Schedule(Then, onComplete);
				else if (onComplete != null)
					return onComplete(context, this);
			}
			else
			{
				if (Else != null)
					context.Schedule(Else, onComplete);
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
