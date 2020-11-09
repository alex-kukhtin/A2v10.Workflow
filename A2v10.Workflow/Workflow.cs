using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Workflow : IWorkflow
	{
		private readonly IActivity _root;
		private readonly Object _args;
		private readonly ExecutionContext _context;

		public static IWorkflow Create(IActivity root, Object args = null)
		{
			var wf = new Workflow(root, args);
			return wf;
		}

		private Workflow(IActivity root, Object args = null)
		{
			_root = root ?? throw new ArgumentNullException(nameof(root));
			_args = args;
			_context = new ExecutionContext(_root, _args);
		}

		#region IWorkflow

		public async ValueTask<ExpandoObject> RunAsync()
		{
			_context.Schedule(_root, null);
			await _context.RunAsync();
			return _context.GetResult();
		}

		public async ValueTask<ExpandoObject> ResumeAsync(String bookmark, Object result)
		{
			await _context.ResumeAsync(bookmark, result);
			await _context.RunAsync();
			return _context.GetResult();
		}
		#endregion
	}
}
