using System;

using A2v10.System.Xaml;
using A2v10.Workflow.Bpmn;


namespace A2v10.Workflow
{
	[ContentProperty("Text")]
	public class GlobalScript : BaseElement
	{
		public String Text { get; init; }
	}
}
