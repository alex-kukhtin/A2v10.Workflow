using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.SqlServer
{
	public class SqlServerWorkflowStorage : IWorkflowStorage
	{
		private readonly IDbContext _dbContext;

		public SqlServerWorkflowStorage(IDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async ValueTask<IWorkflow> LoadAsync(IIdentity identity)
		{
			var prms = new ExpandoObject();

			var eo = await _dbContext.ReadExpandoAsync(null, "a2wf.[Workflow.Load]", prms);

			return new Workflow()
			{
				Identity = new Identity()
				{
					Id = eo.Get<String>("WorkflowId"),
					Version = eo.Get<Int32>("Version")
				},
				Root = null
			};
		}

		public ValueTask<IIdentity> PublishAsync(String id, String text, String format)
		{
			throw new NotImplementedException();
		}
	}
}