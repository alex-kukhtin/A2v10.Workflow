using A2v10.System.Xaml;
using A2v10.Workflow.Bpmn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	/*two classes with same name is required !*/
	[ContentProperty("Text")]
	public class Script : BaseElement
	{
		public String Text { get; init; }
	}
}
