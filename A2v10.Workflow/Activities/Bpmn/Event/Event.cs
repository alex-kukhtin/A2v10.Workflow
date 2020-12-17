using System;
using System.Linq;
using System.Collections.Generic;
using A2v10.System.Xaml;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public abstract class Event : BpmnActivity
	{
		public virtual Boolean IsStart => false;

		public List<BaseElement> Children { get; init; }

		public IEnumerable<Outgoing> Outgoing => Children.OfType<Outgoing>();

		public EventDefinition EventDefinition => Children.OfType<EventDefinition>().FirstOrDefault();
	}
}
