using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class IntermediateCatchEvent : Event
	{
		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			throw new NotImplementedException();
		}
	}
}
