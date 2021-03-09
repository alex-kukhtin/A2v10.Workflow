
using System;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests
{
	class InMemoryPendingInstance : IPendingInstance
	{
		public Guid InstanceId { get; set; }
		public String EventKey { get; set; }
	}
}
