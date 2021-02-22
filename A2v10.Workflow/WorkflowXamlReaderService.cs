using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	public class WorkflowXamlReaderService : XamlReaderService
	{
		public override XamlServicesOptions Options { get; set; }

		public WorkflowXamlReaderService()
		{
			Options = XamlServicesOptions.BpmnXamlOptions;
		}
	}
}
