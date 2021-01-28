
using System;
using A2v10.System.Xaml;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Text")]
	public class TextAnnotation : BaseElement
	{
		public String Text { get; init; }
	}
}
