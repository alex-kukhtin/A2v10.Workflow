
using System;
using System.Dynamic;
using System.Threading.Tasks;
using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.SqlServer
{
	public class SqlServerInstanceStorage : IInstanceStorage
	{
		private readonly IDbContext _dbContext;
		private readonly IWorkflowStorage _workflowStorage;
		private readonly ISerializer _serializer;

		public SqlServerInstanceStorage(IDbContext dbContext, IWorkflowStorage workflowStorage, ISerializer serializer)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_workflowStorage = workflowStorage ?? throw new ArgumentNullException(nameof(workflowStorage));
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}

		#region IInstanceStorage
		public async Task<IInstance> Load(Guid instanceId)
		{
			var prms = new ExpandoObject();
			prms.Set<Guid>("Id", instanceId);
			var eo = await _dbContext.ReadExpandoAsync(null, $"{Definitions.SqlSchema}.[Instance.Load]", prms);
			if (eo == null)
				throw new SqlServerStorageException($"Instance '{instanceId}' not found");

			var identity = new Identity()
			{
				Id = eo.Get<String>("WorkflowId"),
				Version = eo.Get<Int32>("Version")
			};

			return new Instance()
			{
				Id = instanceId,
				Parent = eo.Get<Guid>("Parent"),
				Workflow = await _workflowStorage.LoadAsync(identity),
				State = _serializer.Deserialize(eo.Get<String>("State")),
				ExecutionStatus = Enum.Parse<WorkflowExecutionStatus>(eo.Get<String>("ExecutionStatus")),
				Lock = eo.Get<Guid>("Lock")
			};
		}

		public async Task Create(IInstance instance)
		{
			var ieo = new ExpandoObject();
			ieo.Set("Id", instance.Id);
			ieo.Set("Parent", instance.Parent);
			ieo.Set("Version", instance.Workflow.Identity.Version);
			ieo.Set("WorkflowId", instance.Workflow.Identity.Id);
			ieo.Set("ExecutionStatus", instance.ExecutionStatus.ToString());
			await _dbContext.ExecuteExpandoAsync(null, $"{Definitions.SqlSchema}.[Instance.Create]", ieo);
		}

		public async Task Save(IInstance instance)
		{
			var ieo = new ExpandoObject();
			ieo.Set("Id", instance.Id);
			ieo.Set("WorkflowId", instance.Workflow.Identity.Id);
			ieo.Set("ExecutionStatus", instance.ExecutionStatus.ToString());
			ieo.Set("Lock", instance.Lock);
			ieo.Set("State", _serializer.Serialize(instance.State));
			ieo.Set("Variables", instance.ExternalVariables);
			ieo.Set("Bookmarks", instance.ExternalBookmarks);

			var root = new ExpandoObject();
			root.Set("Instance", ieo);
			await _dbContext.SaveModelAsync(null, $"{Definitions.SqlSchema}.[Instance.Update]", root);
		}

		#endregion
	}
}
