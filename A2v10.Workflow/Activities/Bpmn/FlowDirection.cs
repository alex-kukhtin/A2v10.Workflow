using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{

	[ContentProperty("Text")]
	public abstract class FlowDirection : BpmnItem
	{
		public String Text { get; set; }
		public abstract Boolean IsIncoming { get; }
	}

	public class Incoming : FlowDirection
	{
		public override Boolean IsIncoming => true;
	}

	public class Outgoing : FlowDirection
	{
		public override Boolean IsIncoming => false;
	}
}
