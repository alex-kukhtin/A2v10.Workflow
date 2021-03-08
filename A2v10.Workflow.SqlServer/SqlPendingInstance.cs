
using System;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.SqlServer
{
	public class SqlPendingInstance : IPendingInstance
	{
		public Guid InstanceId { get; set; }
		public string EventKey { get; set; }
	}
}
