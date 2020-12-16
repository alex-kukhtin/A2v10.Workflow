using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class SequenceFlow : BpmnActivity
	{
		public String SourceRef { get; init; }
		public String TargetRef { get; init; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var target = Parent.FindElement<BpmnActivity>(TargetRef);
			context.Schedule(target, onComplete, token);
			return new ValueTask();
		}
	}
}
