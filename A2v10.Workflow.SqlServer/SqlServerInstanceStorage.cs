
using System;
using System.Dynamic;
using System.Threading.Tasks;
using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.SqlServer
{
	public class SqlServerInstanceStorage : IInstanceStorage
	{
		private const String Schema = "[A2v10.Workflow]";

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
			var eo = await _dbContext.ReadExpandoAsync(null, $"{Schema}.[Instance.Load]", prms);
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
				State = _serializer.Deserialize(eo.Get<String>("State"))
			};
		}

		public async Task Save(IInstance instance)
		{
			var prms = new ExpandoObject();
			prms.Set("Id", instance.Id);
			prms.Set("Parent", instance.Parent);
			prms.Set("Version", instance.Workflow.Identity.Version);
			prms.Set("WorkflowId", instance.Workflow.Identity.Id);
			prms.Set("State", _serializer.Serialize(instance.State));
			await _dbContext.ExecuteExpandoAsync(null, $"{Schema}.[Instance.Save]", prms);
		}

		#endregion
	}
}
