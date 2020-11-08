
using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class If : Activity
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

		public override void Traverse(Action<IActivity> onAction)
		{
			base.Traverse(onAction);
			if (Then != null)
				onAction(Then);
			if (Else != null)
				onAction(Else);
		}

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Condition);
			if (cond)
			{
				if (Then != null)
					context.Schedule(Then, onComplete);
			}
			else
			{
				if (Else != null)
					context.Schedule(Else, onComplete);
			}
			return new ValueTask();
		}
	}
}
