using System;

using A2v10.System.Xaml;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Text")]
	public class TargetRef : BaseElement
	{
		public String Text { get; init; }
	}
}
