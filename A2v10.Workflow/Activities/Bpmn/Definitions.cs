using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Children")]
	public class Definitions
	{
		public String Id { get; init; }
		public String TargetNamespace { get; init; }
		public String Exporter { get; set; }
		public String ExporterVersion { get; set; }

		public List<Object> Children { get; init; }

		public Process Process => Children.OfType<Process>().FirstOrDefault();
	}
}
