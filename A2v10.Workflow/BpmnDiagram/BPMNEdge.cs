using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.System.Xaml;

namespace A2v10.Workflow.Bpmn.Diagram
{
	[ContentProperty("Waypoints")]
	public class BPMNEdge : DiagramElement
	{
		public List<waypoint> Waypoints { get; init; }
	}
}
