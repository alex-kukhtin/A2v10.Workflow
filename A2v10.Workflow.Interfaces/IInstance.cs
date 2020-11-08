
using System;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstance
	{
		Guid Id { get; }
		IWorkflow Workflow { get; }
	}
}
