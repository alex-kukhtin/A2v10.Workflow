using System;
using System.Collections.Generic;

namespace A2v10.Workflow.Bpmn
{
	public abstract class Gateway : BpmnElement
	{
		public List<String> Incoming { get; init; }
		public List<String> Outgoing { get; init; }

		public String Default { get; init; }
	}
}
