﻿using A2v10.Workflow.Interfaces;
using System;
using System.Linq;
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
			return ValueTask.CompletedTask;
		}

		SequenceFlow FindFlowToExecute(IExecutionContext context)
		{
			if (!HasOutgoing)
				return null;
			if (Outgoing.Count() == 1)
			{
				// join flows
				return Parent.FindElement<SequenceFlow>(Outgoing.ElementAt(0).Text);
			}

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
