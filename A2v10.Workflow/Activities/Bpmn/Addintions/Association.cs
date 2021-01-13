using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class Association : BaseElement
	{
		public String SourceRef { get; init; }
		public String TargetRef { get; init; }

	}
}
