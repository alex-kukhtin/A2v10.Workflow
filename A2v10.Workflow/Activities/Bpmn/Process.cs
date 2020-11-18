using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class Process : BpmnElement
	{
		public Boolean IsExecutable { get; init; }

		public List<BpmnElement> Elements { get; init; }
	}
}
