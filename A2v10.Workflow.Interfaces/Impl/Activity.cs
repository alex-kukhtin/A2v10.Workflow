using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class Activity : IActivity
	{
		#region IActivity
		public String Ref { get; set; }

		public abstract ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete);

		public virtual IEnumerable<IActivity> EnumChildren()
		{
			return Enumerable.Empty<IActivity>();
		}

		public ValueTask CancelAsync(IExecutionContext context)
		{
			return this.TraverseAsync(act => act.CancelAsync(context));
		}

		public virtual void OnEndInit()
		{
			foreach (var act in EnumChildren())
				act.OnEndInit();
		}
		#endregion
	}
}
