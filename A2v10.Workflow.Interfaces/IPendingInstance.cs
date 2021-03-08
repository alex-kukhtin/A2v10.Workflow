
using System;

namespace A2v10.Workflow.Interfaces
{
	public interface IPendingInstance
	{
		Guid InstanceId { get; }
		String EventKey { get; }
	}
}
