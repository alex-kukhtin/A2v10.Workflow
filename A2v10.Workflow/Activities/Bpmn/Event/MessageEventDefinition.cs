
using System;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	class MessageEventDefinition : EventDefinition
	{
		public override IWorkflowEvent CreateEvent(string id)
		{
			throw new NotImplementedException("MessageEventDefinition.CreateEvent");
		}
	}
}
