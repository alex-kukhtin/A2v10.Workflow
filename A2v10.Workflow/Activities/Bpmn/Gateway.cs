using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public abstract class Gateway : BpmnElement
	{
		public List<String> Incoming { get; set; }
		public List<String> Outgoing { get; set; }

		public String Default { get; set; }
	}
}
