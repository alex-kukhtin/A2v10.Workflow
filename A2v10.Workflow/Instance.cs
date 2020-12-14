
using System;
using System.Dynamic;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Instance : IInstance
	{
		public IWorkflow Workflow { get; init; }

		public Guid Id { get; init; }
		public Guid? Parent { get; init; }

		public ExpandoObject Result { get; set; }
		public ExpandoObject State { get; set; }
	}
}
