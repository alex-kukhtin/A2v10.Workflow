
using System;
using System.Collections.Generic;
using System.Dynamic;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Instance : IInstance
	{
		public IWorkflow Workflow { get; init; }

		public Guid Id { get; init; }
		public Guid? Parent { get; init; }

		public WorkflowExecutionStatus ExecutionStatus { get; set; }
		public Guid? Lock { get; init; }

		public ExpandoObject Result { get; set; }
		public ExpandoObject State { get; set; }

		public ExpandoObject ExternalVariables { get; set; }
		public ExpandoObject ExternalBookmarks { get; set; }
	}
}
