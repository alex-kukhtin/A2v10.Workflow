
using System;

namespace A2v10.Workflow.Bpmn
{
	public class TimeDate : TimeBase
	{
		public override Boolean CanRepeat => false;
		public override DateTime NextTriggerTime => throw new NotImplementedException("TimeDate.NextTriggerTime");
	}
}
