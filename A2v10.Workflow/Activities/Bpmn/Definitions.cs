using A2v10.System.Xaml;
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public class Definitions : IActivityWrapper
	{
		public String Id { get; init; }
		public String Name { get; init; }
		public String TargetNamespace { get; init; }
		public String Exporter { get; set; }
		public String ExporterVersion { get; set; }
		public String ExpressionLanguage { get; set; }
		public String TypeLanguage { get; set; }

		public List<Object> Children { get; init; }

		public Process Process => Children.OfType<Process>().FirstOrDefault();

		public IActivity Root => Process;
	}
}
