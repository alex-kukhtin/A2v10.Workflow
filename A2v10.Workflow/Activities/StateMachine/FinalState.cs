
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;
using Microsoft.VisualBasic;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class FinalState : StateBase
	{
		public override Boolean IsFinal => false;

		public IActivity Entry { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			context.Schedule(Entry, onComplete);
			return new ValueTask();
		}

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Entry != null)
				yield return Entry;
		}

	}
}
