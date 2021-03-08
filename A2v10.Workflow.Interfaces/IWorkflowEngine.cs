
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflowEngine
	{
		ValueTask<IInstance> CreateAsync(IIdentity identity);
		ValueTask<IInstance> CreateAsync(IActivity root, IIdentity identity);

		ValueTask<IInstance> RunAsync(Guid id, Object args = null);
		ValueTask<IInstance> RunAsync(IInstance instance, Object args = null);

		ValueTask<IInstance> ResumeAsync(Guid id, String bookmark, Object reply = null);
		ValueTask<IInstance> HandleEventAsync(Guid id, String eventKey, Object reply = null);

		ValueTask ProcessPending();
	}
}
