
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflowEngine
	{
		ValueTask<IInstance> StartAsync(IActivity root, IIdentity identity, Object args = null);
		ValueTask<IInstance> StartAsync(IIdentity identity, Object args = null);
		ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null);
	}
}
