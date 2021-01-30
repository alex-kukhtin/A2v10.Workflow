using A2v10.Workflow.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class ServiceTask : BpmnTask
	{
		public override ValueTask ExecuteBody(IExecutionContext context)
		{
			throw new NotImplementedException();
		}
	}
}
