using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public record NamespaceDef(String Name, Boolean IsCamelCase, String Namespace, String Assembly);

	public class XamlServicesOptions
	{
		public NamespaceDef[] Namespaces { get; init; }
		public Dictionary<String, String> Aliases { get; init; }
		public Boolean DisableMarkupExtensions { get; init; }


		private static readonly NamespaceDef[] BPMNNamespaces = new NamespaceDef[] {
			new NamespaceDef("http://www.omg.org/spec/bpmn/20100524/model", true, "A2v10.Workflow.Bpmn", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/bpmn/20100524/di", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/dd/20100524/di", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/dd/20100524/dc", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow"),
			new NamespaceDef("http://www.w3.org/2001/xmlschema-instance", true, "A2v10.Workflow.Bpmn", "A2v10.Workflow"),
		};

		public static XamlServicesOptions BpmnXamlOptions =>
			new XamlServicesOptions()
			{
				Namespaces = BPMNNamespaces,
				Aliases = new Dictionary<String, String>() {
					{ "Task", "BpmnTask" }
				},
				DisableMarkupExtensions = true
			};
	}
}
