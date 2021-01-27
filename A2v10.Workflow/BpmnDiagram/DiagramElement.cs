using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn.Diagram
{
	[ContentProperty("Elements")]
	public class DiagramElement : BaseElement
	{
#pragma warning disable IDE1006 // Naming Styles
		public String id { get; init; }
		public String bpmnElement {get;init;}
#pragma warning restore IDE1006 // Naming Styles

		public List<DiagramElement> Elements { get; init; }
	}
}
