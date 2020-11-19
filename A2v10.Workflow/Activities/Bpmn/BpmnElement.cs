using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class BpmnElement : IActivity
	{
		public String Id { get; init; }
		public String Name { get; init; }

		protected Process Parent { get; private set; }

		#region IActivity

		public ValueTask CancelAsync(IExecutionContext context)
		{
			return this.TraverseAsync(act => act.CancelAsync(context));
		}

		public virtual IEnumerable<IActivity> EnumChildren()
		{
			return Enumerable.Empty<IActivity>();
		}

		public abstract ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete);

		public virtual void OnEndInit()
		{
			foreach (var act in EnumChildren())
				act.OnEndInit();
		}
		#endregion

		public void SetParent(Process parent)
		{
			Parent = parent;
		}
	}
}
