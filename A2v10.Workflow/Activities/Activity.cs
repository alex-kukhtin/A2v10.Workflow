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

		public abstract ValueTask Execute(IExecutionContext context, ExecutingAction onComplete);

		public virtual ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			return onAction(this);
		}

		public virtual void Traverse(Action<IActivity> onAction)
		{
			onAction(this);
		}

		public ValueTask Cancel(IExecutionContext context)
		{
			Traverse(act => act.Cancel(context));
			return new ValueTask();
		}

		#endregion
	}
}
