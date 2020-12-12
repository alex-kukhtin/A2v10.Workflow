
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflowStorage
	{
		ValueTask<IWorkflow> LoadAsync(IIdentity identity);
		ValueTask<IIdentity> PublishAsync(String id, String text, String format);
	}
}
