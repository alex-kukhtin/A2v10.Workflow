using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Workflow : IWorkflow
	{
		private readonly IActivity _root;

		public Workflow(IActivity root)
		{
			_root = root ?? throw new ArgumentNullException(nameof(root));
		}

		#region IWorkflow
		public async ValueTask RunAsync()
		{
			var ec = new ExecutionContext();
			ec.BuildScript(_root);
			ec.Schedule(_root, null);
			await ec.RunAsync();
		}

		public ValueTask ResumeAsync(string bookmark)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
