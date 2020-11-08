﻿using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Code : Activity
	{
		public String Script { get; set; }

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			context.Execute(Script);
			return onComplete.Invoke(context, this);
		}
	}
}
