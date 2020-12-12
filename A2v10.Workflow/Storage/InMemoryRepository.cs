using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Storage
{
	public class InMemoryRepository : IRepository
	{
		public IInstanceStorage InstanceStorage { get; }
		public IWorkflowStorage WorkflowStorage { get; }

		public InMemoryRepository(IInstanceStorage instanceStorage, IWorkflowStorage workflowStorage)
		{
			InstanceStorage = instanceStorage;
			WorkflowStorage = workflowStorage;
		}
	}
}
