using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class BaseElement
	{
		public String Id { get; init; }

		public List<BaseElement> Children { get; init; }

		public IEnumerable<TResult> ExtensionElements<TResult>()
		{
			if (this.Children == null)
				return null;
			var ee = this.Children.OfType<ExtensionElements>().FirstOrDefault();
			if (ee == null)
				return null;
			return ee.Items.OfType<TResult>();
		}
	}
}
