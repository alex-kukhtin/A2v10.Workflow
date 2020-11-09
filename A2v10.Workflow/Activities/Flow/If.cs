
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

		public override async ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			await base.TraverseAsync(onAction);
			if (Then != null)
				await onAction(Then);
			if (Else != null)
				await onAction(Else);
		}

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Then != null)
				yield return Then;
			if (Else != null)
				yield return Else;
		}
		public override void Traverse(TraverseArg traverse)
		{
			traverse.Start?.Invoke(this);
			traverse.Action?.Invoke(this);
			Then?.Traverse(traverse);
			Else?.Traverse(traverse);
			traverse.End?.Invoke(this);
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Ref, nameof(Condition));
			if (cond)
				context.Schedule(Then, onComplete);
			else
				context.Schedule(Else, onComplete);
			return new ValueTask();
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildEvaluate(nameof(Condition), Condition);
		}
		#endregion
	}
}
