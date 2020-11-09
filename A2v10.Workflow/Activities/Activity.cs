using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class Activity : IActivity
	{
		#region IActivity
		public String Ref { get; set; }

		public abstract ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete);

		public virtual void Traverse(TraverseArg traverse)
		{
			traverse.Start?.Invoke(this);
			traverse.Action?.Invoke(this);
			traverse.End?.Invoke(this);
		}

		public virtual ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			if (onAction != null)
				return onAction(this);
			return new ValueTask();
		}


		public ValueTask CancelAsync(IExecutionContext context)
		{
			return TraverseAsync(act => act.CancelAsync(context));
		}

		#endregion
	}
}
