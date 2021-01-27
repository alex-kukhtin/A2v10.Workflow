using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class ExclusiveGateway : Gateway
	{
		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			SequenceFlow flowToExecute = FindFlowToExecute(context);
			if (flowToExecute != null)
				context.Schedule(flowToExecute, null, token);
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}

		SequenceFlow FindFlowToExecute(IExecutionContext context)
		{
			if (!HasOutgoing)
				return null;
			foreach (var og in Outgoing)
			{
				var flow = Parent.FindElement<SequenceFlow>(og.Text);
				if (flow != null)
				{
					if (flow.Evaluate(context))
						return flow;
				}
			}
			if (!String.IsNullOrEmpty(Default))
				return Parent.FindElement<SequenceFlow>(Default);
			return null;
		}
	}
}
