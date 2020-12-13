using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public abstract class FlowElement : BpmnElement
	{
		public List<FlowDirection> Children { get; init; }

		public String Default { get; init; }

		public Boolean HasIncoming => Children != null && Children.Any(x => x.IsIncoming);
		public Boolean HasOutgoing => Children != null && Children.Any(x => !x.IsIncoming);

		internal IEnumerable<FlowDirection> Incoming => Children?.Where(x => x.IsIncoming);
		internal IEnumerable<FlowDirection> Outgoing => Children?.Where(x => !x.IsIncoming);
	}
}
