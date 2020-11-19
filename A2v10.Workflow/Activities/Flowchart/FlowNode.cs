using A2v10.Workflow.Interfaces;
using System;

namespace A2v10.Workflow
{
	public abstract class FlowNode : Activity
	{
		public virtual Boolean IsStart => false;

		public Flowchart Parent { get; set; }

		public String Next { get; set; }
	}
}
