
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class State : Activity
	{
		public virtual Boolean IsStart => false;
		public virtual Boolean IsFinal => false;

		public IActivity Trigger { get; set; }
		public IActivity Entry { get; set; }
		public IActivity Exit { get; set; }

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			return new ValueTask();
		}
	}
}
