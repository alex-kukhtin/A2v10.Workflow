using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class EventDefinition : BaseElement
	{
		public virtual ValueTask ExecuteAsync(IExecutionContext context)
		{
			return ValueTask.CompletedTask;
		}
	}
}
