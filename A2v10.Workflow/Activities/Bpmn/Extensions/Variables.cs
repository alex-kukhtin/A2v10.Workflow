using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.System.Xaml;
using A2v10.Workflow.Bpmn;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	[ContentProperty("Items")]

	public class Variables : BaseElement
	{
		public List<Variable> Items { get; init; }
	}
}
