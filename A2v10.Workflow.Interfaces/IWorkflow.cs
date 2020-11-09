
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IWorkflow
	{
		ValueTask<ExpandoObject> RunAsync();
		ValueTask<ExpandoObject> ResumeAsync(String bookmark, Object reply);
	}
}
