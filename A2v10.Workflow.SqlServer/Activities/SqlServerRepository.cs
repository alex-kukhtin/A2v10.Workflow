using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.SqlServer.Activities
{
	public class SqlServerRepository : IRepository
	{
		public IWorkflowStorage WorkflowStorage { get; }
		public IInstanceStorage InstanceStorage { get; }

		public SqlServerRepository(IWorkflowStorage workflowStorage, IInstanceStorage instanceStorage)
		{
			WorkflowStorage = workflowStorage;
			InstanceStorage = instanceStorage;
		}
	}
}
