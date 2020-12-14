using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	public class ActivityTask : BpmnTask
	{
		IActivity Activity { get; init; }

		public override IEnumerable<IActivity> EnumChildren()
		{
			yield return Activity;
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			context.Schedule(Activity, onComplete, token);
			return new ValueTask();
		}
	}
}
