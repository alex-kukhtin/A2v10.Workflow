
using System;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	public class SignalEventDefinition : EventDefinition
	{
		public override IWorkflowEvent CreateEvent(string id)
		{
			throw new NotImplementedException("SignalEventDefinition.CreateEvent");
		}
	}
}
