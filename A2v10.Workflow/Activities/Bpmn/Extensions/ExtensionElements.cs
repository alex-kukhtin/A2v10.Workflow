using A2v10.System.Xaml;
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Items")]
	public class ExtensionElements : BaseElement
	{
		public List<BaseElement> Items { get; init; }

		public List<IVariable> GetVariables()
		{
			return Items.OfType<Variables>().FirstOrDefault()?.Items.OfType<IVariable>().ToList();
		}
	}
}
