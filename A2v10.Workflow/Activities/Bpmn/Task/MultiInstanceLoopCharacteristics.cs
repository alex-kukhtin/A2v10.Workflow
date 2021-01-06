using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class MultiInstanceLoopCharacteristics : BaseElement
	{
		public Boolean IsSequential { get; init; }
		public String LoopCardinality { get; init; }
		public String LoopDataInputRef { get; init; }
		public String LoopDataOutputRef { get; init; }
		public String InputDataItem { get; init; }
	}
}
