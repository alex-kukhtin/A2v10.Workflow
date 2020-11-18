
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	public class Bpmn : IActivity
	{
		public Process Process { get; init; }

		#region IActivity
		public String Ref { get; init; }

		public ValueTask CancelAsync(IExecutionContext context)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IActivity> EnumChildren()
		{
			throw new NotImplementedException();
		}

		public ValueTask ExecuteAsync(IExecutionContext context, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			throw new NotImplementedException();
		}

		public void OnEndInit()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
