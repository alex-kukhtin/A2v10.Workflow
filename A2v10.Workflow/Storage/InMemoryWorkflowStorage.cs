using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Storage
{
	public record StoredWorkflow
	{
		public String Text { get; set; }
		public String Format { get; set; }
		public String WorkflowId { get; set; }
		public Int32 Version { get; set; }
	}

	public class InMemoryWorkflowStorage : IWorkflowStorage
	{
		private readonly List<StoredWorkflow> _storage = new List<StoredWorkflow>();
		private readonly ISerializer _serializer;

		public InMemoryWorkflowStorage(ISerializer serializer)
		{
			_serializer = serializer;
		}

		public ValueTask<IWorkflow> LoadAsync(IIdentity identity)
		{
			Int32 v = identity.Version;
			if (v == 0)
			{
				// find max version
				v = _storage.FindAll(sw => sw.WorkflowId == identity.Id).Max(x => x.Version);
			}
			var swf = _storage.Find(x => x.WorkflowId == identity.Id && x.Version == v);
			if (swf == null)
				throw new KeyNotFoundException($"Workflow '{identity}' not found");
			var root = _serializer.DeserializeActitity(swf.Text, swf.Format);
			var wf = new Workflow()
			{
				Identity = new Identity()
				{
					Id = identity.Id,
					Version = v
				},
				Root = root
			};
			return new ValueTask<IWorkflow>(wf);
		}
	}
}
