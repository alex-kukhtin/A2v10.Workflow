
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class FlowDecision : FlowNode, IScriptable
	{
		public String Condition { get; set; }
		public String Then { get; set; }
		public String Else { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			var cond = context.Evaluate<Boolean>(Ref, nameof(Condition));
			var nextNode = Parent.FindNode(cond ? Then : Else);
			if (nextNode == null)
				nextNode = Parent.FindNode(Next);
			if (nextNode != null)
				context.Schedule(nextNode, onComplete);
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
