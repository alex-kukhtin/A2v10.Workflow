
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflowStorage
	{
		Task<IWorkflow> LoadAsync(IIdentity identity);
		Task<String> LoadSourceAsync(IIdentity identity);
		Task<IIdentity> PublishAsync(IWorkflowCatalog catalog, String id);
	}
}
