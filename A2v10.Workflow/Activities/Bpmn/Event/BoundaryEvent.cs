﻿using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BoundaryEvent : Event
	{
		public String AttachedToRef { get; init; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			throw new NotImplementedException();
		}
	}
}