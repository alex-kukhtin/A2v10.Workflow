using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Storage
{
	internal record SavedInstance(IActivity Root, String State);

	public class InMemoryInstanceStorage : IInstanceStorage
	{

		private readonly Dictionary<Guid, SavedInstance> _memory = new();

		private readonly ISerializer _serializer;
		public InMemoryInstanceStorage(ISerializer serializer)
		{
			_serializer = serializer;
		}

		public Task<IInstance> Load(Guid id)
		{
			if (_memory.TryGetValue(id, out SavedInstance saved))
			{
				IInstance inst = new Instance()
				{
					Id = id,
					Workflow = new Workflow() { Root = saved.Root },
					State = _serializer.Deserialize(saved.State)
				};
				return Task.FromResult(inst);
			}
			throw new NotImplementedException();
		}

		public Task Create(IInstance instance)
		{
			if (_memory.ContainsKey(instance.Id))
				throw new WorkflowException($"Instance storage. Instance with id = {instance.Id} has been already created");
			var si = new SavedInstance(instance.Workflow.Root, _serializer.Serialize(instance.State));
			_memory.Add(instance.Id, si);
			return Task.CompletedTask;
		}

		public Task Save(IInstance instance)
		{
			if (_memory.ContainsKey(instance.Id))
			{
				var si = new SavedInstance(instance.Workflow.Root, _serializer.Serialize(instance.State));
				_memory[instance.Id] = si;
			}
			else
				throw new WorkflowException($"Instance storage. Instance with id = {instance.Id} not found");
			return Task.CompletedTask;
		}

		public Task WriteException(Guid id, Exception ex)
		{
			return Task.CompletedTask;
		}

		public Task<IEnumerable<IPendingInstance>> GetPendingAsync()
		{
			return Task.FromResult(Enumerable.Empty<IPendingInstance>());
		}
	}
}
