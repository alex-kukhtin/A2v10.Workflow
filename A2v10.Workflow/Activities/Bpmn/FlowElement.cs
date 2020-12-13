using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public abstract class FlowElement : BpmnElement
	{
		public List<BpmnItem> Children { get; init; }

		public String Default { get; init; }

		public Boolean HasIncoming => Children != null && Children.OfType<Incoming>().Any();
		public Boolean HasOutgoing => Children != null && Children.OfType<Outgoing>().Any();

		internal IEnumerable<Incoming> Incoming => Children?.OfType<Incoming>();
		internal IEnumerable<Outgoing> Outgoing => Children?.OfType<Outgoing>();
	}
}
