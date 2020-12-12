using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class FlowStart : FlowNode
	{
		public override bool IsStart => true;

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var node = Parent.FindNode(Next);
			if (node != null)
				context.Schedule(node, onComplete, token);
			return new ValueTask();
		}
	}
}
