using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow
{
	public sealed class WorkflowExecption : Exception
	{
		public WorkflowExecption(String message)
			: base(message)
		{

		}
	}
}
