
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Workflow : IWorkflow
	{
		public IIdentity Identity { get; init; }
		public IActivity Root { get; init; }
	}
}
