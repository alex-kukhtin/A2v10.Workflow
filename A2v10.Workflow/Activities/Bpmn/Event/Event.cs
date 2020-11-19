using System;

namespace A2v10.Workflow.Bpmn
{
	public abstract class Event : BpmnElement
	{
		public virtual Boolean IsStart => false;
	}
}
