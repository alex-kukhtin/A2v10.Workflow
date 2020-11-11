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
		public SqlServerInstanceStorage(IDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		#region IInstanceStorage
		public async Task<IInstance> Load(Guid instanceId)
		{
			ExpandoObject prms = null;
			var eo = await _dbContext.ReadExpandoAsync(null, $"{Schema}.[Instance.Load]", prms);
			if (eo == null)
				throw new SqlServerStorageException($"Instance '{instanceId}' not found");
			return null;
		}

		public async Task Save(IInstance instance)
		{
			var prms = new ExpandoObject();
			await _dbContext.ExecuteExpandoAsync(null, $"{Schema}.[Instance.Save]", prms);
		}
		#endregion
	}
}
