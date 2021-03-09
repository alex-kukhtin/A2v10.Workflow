﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests
{
	internal record SavedInstance(IActivity Root, String State, IInstanceData InstanceData, ExpandoObject Result, WorkflowExecutionStatus Status);

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
					State = _serializer.Deserialize(saved.State),
					Result = saved.Result,
					ExecutionStatus = saved.Status
				};
				return Task.FromResult(inst);
			}
			throw new NotImplementedException();
		}

		public Task Create(IInstance instance)
		{
			if (_memory.ContainsKey(instance.Id))
				throw new WorkflowException($"Instance storage. Instance with id = {instance.Id} has been already created");
			var si = new SavedInstance(instance.Workflow.Root, 
				_serializer.Serialize(instance.State), instance.InstanceData, 
				instance.Result, instance.ExecutionStatus);
			_memory.Add(instance.Id, si);
			return Task.CompletedTask;
		}

		public Task Save(IInstance instance)
		{
			if (_memory.ContainsKey(instance.Id))
			{
				var si = new SavedInstance(instance.Workflow.Root, 
					_serializer.Serialize(instance.State), instance.InstanceData, 
					instance.Result, instance.ExecutionStatus);
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

		private static String IsTimerExpired(List<Object> events)
		{
			if (events == null)
				return null;
			var now = DateTime.UtcNow + TimeSpan.FromSeconds(2); // for testing propouses
			foreach (var t in events)
			{
				if (t is ExpandoObject eo)
				{
					var kind = eo.Get<String>("Kind");
					if (kind == "T")
					{
						var exp = eo.Get<DateTime>("Pending");
						if (exp <= now)
							return eo.Get<String>("Event");
					}
				}
			}
			return null;
		}

		public Task<IEnumerable<IPendingInstance>> GetPendingAsync()
		{
			// timers
			var list = new List<IPendingInstance>();
			foreach (var (k, v) in _memory)
			{
				var eventKey = IsTimerExpired(v.InstanceData.ExternalEvents);
				if (!String.IsNullOrEmpty(eventKey))
					list.Add(new InMemoryPendingInstance() { InstanceId = k, EventKey = eventKey });
			}
			return Task.FromResult<IEnumerable<IPendingInstance>>(list);
		}
	}
}
