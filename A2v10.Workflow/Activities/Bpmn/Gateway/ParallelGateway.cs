using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class ParallelGateway : Gateway
	{
		private readonly List<IToken> _tokens = new List<IToken>();

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			// waits for all incoming tokens
			_tokens.Add(token);
			if (HasIncoming && _tokens.Count == Incoming.Count)
				return DoOutgoing(context, onComplete);
			else
				return new ValueTask();
		}

		public ValueTask DoOutgoing(IExecutionContext context, ExecutingAction onComplete)
		{
			// kill all tokens
			foreach (var t in _tokens)
				Parent.KillToken(t);
			if (HasOutgoing)
			{
				foreach (var og in Outgoing) {
					var flow = Parent.FindElement<SequenceFlow>(og);
					context.Schedule(flow, onComplete, Parent.NewToken());
				}
			}
			return new ValueTask();
		}
	}
}
