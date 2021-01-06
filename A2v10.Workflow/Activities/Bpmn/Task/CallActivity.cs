using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	public class CallActivity : BpmnTask
	{
		public String CalledElement { get; init; }

		public override ValueTask ExecuteBody(IExecutionContext context)
		{
			return base.ExecuteBody(context);
		}

		protected override ValueTask CompleteBody(IExecutionContext context)
		{
			return base.CompleteBody(context);
		}
	}
}
