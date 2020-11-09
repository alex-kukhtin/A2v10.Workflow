using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow
{
	public abstract class FlowNode : Activity
	{
		public String Next { get; set; }
	}
}
