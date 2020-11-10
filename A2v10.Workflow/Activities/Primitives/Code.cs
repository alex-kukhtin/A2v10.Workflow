using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Code : Activity, IScriptable
	{
		public String Script { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			context.Execute(Ref, nameof(Script));
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecute(nameof(Script), Script);
		}
		#endregion
	}
}
