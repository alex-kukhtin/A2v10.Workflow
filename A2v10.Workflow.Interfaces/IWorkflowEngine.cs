
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflowEngine
	{
		ValueTask<IInstance> StartAsync();
		ValueTask<IInstance> ResumeAsync(Guid instanceId, String bookmark);
	}
}
