using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class TimerEventDefinition : EventDefinition
	{
		TimeBase TimeBase => Children.OfType<TimeBase>().FirstOrDefault();

		public override IWorkflowEvent CreateEvent(String id)
		{
			var timeBase = TimeBase;
			if (timeBase == null)
				throw new WorkflowException($"TimerEventDefinition. There is no trigger time for '{Id}'");
			return new WorkflowTimerEvent(id, timeBase.NextTriggerTime);
		}

		public override Boolean CanRepeat => TimeBase != null && TimeBase.CanRepeat;
	}
}
