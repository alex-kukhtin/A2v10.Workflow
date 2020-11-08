
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflow
	{
		ValueTask RunAsync();
		ValueTask ResumeAsync(String bookmark);
	}
}
