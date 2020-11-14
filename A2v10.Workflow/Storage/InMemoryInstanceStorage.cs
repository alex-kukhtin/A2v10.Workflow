using A2v10.Workflow.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Storage
{
	internal record SavedInstance(IActivity Root, String State);

	public class InMemoryInstanceStorage : IInstanceStorage
	{
		private readonly Dictionary<Guid, SavedInstance> _memory = new Dictionary<Guid, SavedInstance>();

		public Task<IInstance> Load(Guid id)
		{
			if (_memory.TryGetValue(id, out SavedInstance saved))
			{
				IInstance inst = new Instance() {
					Id = id,
					Root = saved.Root,
					State = JsonConvert.DeserializeObject<ExpandoObject>(saved.State)
				};
				return Task.FromResult(inst);
			}
			throw new NotImplementedException();
		}

		public Task Save(IInstance instance)
		{
			Console.WriteLine(JsonConvert.SerializeObject(instance.State, new DoubleConverter()));
			
			var si = new SavedInstance(instance.Root, JsonConvert.SerializeObject(instance.State));

			if (_memory.ContainsKey(instance.Id))
				_memory[instance.Id] = si;
			else
				_memory.Add(instance.Id, si);
			return Task.CompletedTask;
		}
	}
}
