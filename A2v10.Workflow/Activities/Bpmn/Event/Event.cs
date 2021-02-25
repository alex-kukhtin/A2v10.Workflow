using System;
using System.Linq;
using System.Collections.Generic;
using A2v10.System.Xaml;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public abstract class Event : BpmnActivity, IScriptable
	{
		public virtual Boolean IsStart => false;

		public IEnumerable<Outgoing> Outgoing => Children?.OfType<Outgoing>();

		public EventDefinition EventDefinition => Children?.OfType<EventDefinition>().FirstOrDefault();

		// wf:Script here
		public String Script => ExtensionElements<A2v10.Workflow.Script>()?.FirstOrDefault()?.Text;

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecute(nameof(Script), Script);
		}
		#endregion
	}
}
