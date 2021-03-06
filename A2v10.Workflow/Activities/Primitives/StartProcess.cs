﻿using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StartProcess : Activity, IScriptable
	{

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			return onComplete.Invoke(context, this);
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
		}
		#endregion
	}
}
