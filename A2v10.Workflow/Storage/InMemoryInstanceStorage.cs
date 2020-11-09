using A2v10.Workflow.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Storage
{
	internal class SavedInstance
	{
		public IActivity Root { get; set; }
		public String State { get; set; }
	}

	public class InMemoryInstanceStorage : IInstanceStorage
	{
		private Dictionary<Guid, SavedInstance> _memory = new Dictionary<Guid, SavedInstance>();

		public Task<IInstance> Load(Guid id)
		{
			if (_memory.TryGetValue(id, out SavedInstance saved))
			{
				var inst = new Instance();
				inst.Id = id;
				inst.Root = saved.Root;
				inst.State = JsonConvert.DeserializeObject<ExpandoObject>(saved.State);
				return Task.FromResult(inst as IInstance);
			}
			throw new NotImplementedException();
		}

		public Task Save(IInstance instance)
		{
			Console.WriteLine("Save Instance");
			Console.WriteLine(JsonConvert.SerializeObject(instance.State));
			var si = new SavedInstance();
			si.Root = instance.Root;
			si.State = JsonConvert.SerializeObject(instance.State);
			_memory.Add(instance.Id, si);
			return Task.CompletedTask;
		}
	}
}
