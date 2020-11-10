using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Activities
{
	public class FlowStart : FlowNode
	{
		public override bool IsStart => true;

		public override ValueTask ExecuteAsync(IExecutionContext context, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			var node = Parent.FindNode(Next);
			if (node != null)
				context.Schedule(node, onComplete);
			return new ValueTask();
		}
	}
}
