using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn.Diagram
{
	[ContentProperty("Bounds")]
	public class BPMNShape : DiagramElement
	{
		public Bounds Bounds { get; init; }
		public Boolean isMarkerVisible {get; init;}
	}
}
