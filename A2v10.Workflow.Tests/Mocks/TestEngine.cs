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

		public static IServiceProvider ServiceProvider()
		{
			if (_provider != null)
				return _provider;

			var collection = new ServiceCollection();

			collection.AddSingleton<IWorkflowStorage, InMemoryWorkflowStorage>();
			collection.AddSingleton<IInstanceStorage, InMemoryInstanceStorage>();
			collection.AddSingleton<IWorkflowCatalog, InMemoryWorkflowCatalog>();
			collection.AddSingleton<ISerializer, Serializer>();
			collection.AddTransient<IWorkflowEngine, WorkflowEngine>();
			collection.AddTransient<ITracker, ConsoleTracker>();
			//collection.AddTransient<IServiceProvider>(sp => _provider);

			_provider = collection.BuildServiceProvider();

			return _provider;
		}
	}
}
