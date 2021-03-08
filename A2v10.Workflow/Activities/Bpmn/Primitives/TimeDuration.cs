using System;

namespace A2v10.Workflow.Bpmn
{
	public class TimeDuration : TimeBase
	{
		public override Boolean CanRepeat => false;
		public override DateTime NextTriggerTime => DateTime.UtcNow + TimeSpan.Parse(Expression);
	}
}
