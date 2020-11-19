using System;
using System.Collections.Generic;

namespace A2v10.Workflow.Bpmn
{
	public abstract class Gateway : BpmnElement
	{
		public List<String> Incoming { get; init; }
		public List<String> Outgoing { get; init; }

		public String Default { get; init; }

		public Boolean HasIncoming => Incoming != null && Incoming.Count > 0;
		public Boolean HasOutgoing => Outgoing != null && Outgoing.Count > 0;
	}
}
