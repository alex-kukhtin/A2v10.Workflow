using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class SignalStartEvent : StartEvent
	{
		public SignalEventDefinition SignalEventDefinition => Children.OfType<SignalEventDefinition>().FirstOrDefault();
	}
}
