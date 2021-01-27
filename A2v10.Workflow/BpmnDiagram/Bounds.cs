using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn.Diagram
{
	public class Bounds : DiagramElement
	{
#pragma warning disable IDE1006 // Naming Styles
		public Int32 x { get; init; }
		public Int32 y { get; init; }
		public Int32 width { get; init; }
		public Int32 height { get; init; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
