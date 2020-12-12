using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Serialization;
using A2v10.Workflow.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.Workflow.Tests
{
	public class TestEngine
	{
		public static IWorkflowEngine CreateInMemoryEngine()
		{
			return ServiceProvider().GetService<IWorkflowEngine>();
		}

		private static IServiceProvider _provider;

		private static IServiceProvider ServiceProvider()
		{
			if (_provider != null)
				return _provider;

			var collection = new ServiceCollection();

			collection.AddTransient<IRepository, InMemoryRepository>();
			collection.AddTransient<IWorkflowEngine, WorkflowEngine>();
			collection.AddTransient<IWorkflowStorage, InMemoryWorkflowStorage>();
			collection.AddTransient<IInstanceStorage, InMemoryInstanceStorage>();
			collection.AddTransient<ITracker, ConsoleTracker>();
			collection.AddTransient<ISerializer, Serializer>();

			_provider = collection.BuildServiceProvider();
			return _provider;
		}
	}
}
