using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn.Diagram
{
	[ContentProperty("Planes")]
	public class BPMNDiagram : DiagramElement
	{
		public List<BPMNPlane> Planes { get; init; }
	}
}
