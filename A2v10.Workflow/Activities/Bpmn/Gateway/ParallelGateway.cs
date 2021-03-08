﻿using System;
using System.Linq;

using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class ParallelGateway : Gateway
	{
		private readonly List<IToken> _tokens = new();

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			// waits for all incoming tokens
			_tokens.Add(token);
			if (HasIncoming && _tokens.Count == Incoming.Count())
				return DoOutgoing(context, onComplete);
			else
				return ValueTask.CompletedTask;
		}

		public ValueTask DoOutgoing(IExecutionContext context, ExecutingAction onComplete)
		{
			// kill all tokens
			foreach (var t in _tokens)
				Parent.KillToken(t);
			if (HasOutgoing)
			{
				foreach (var og in Outgoing) {
					var flow = Parent.FindElement<SequenceFlow>(og.Text);
					context.Schedule(flow, null, Parent.NewToken());
				}
			}
			if (onComplete != null)
				return onComplete(context, this);
			return ValueTask.CompletedTask;
		}
	}
}
