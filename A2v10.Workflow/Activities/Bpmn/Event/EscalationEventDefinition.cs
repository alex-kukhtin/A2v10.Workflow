
using System;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	public class EscalationEventDefinition : EventDefinition
	{
		public override IWorkflowEvent CreateEvent(string id)
		{
			throw new NotImplementedException("EscalationEventDefinition.CreateEvent");
		}
	}
}
