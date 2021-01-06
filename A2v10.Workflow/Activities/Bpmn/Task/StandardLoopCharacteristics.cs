using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class StandardLoopCharacteristics : BaseElement
	{
		public String LoopCondition { get; init; }
		public Boolean TestBefore { get; init; }
		public Int32 LoopMaximum { get; init; }
	}
}
