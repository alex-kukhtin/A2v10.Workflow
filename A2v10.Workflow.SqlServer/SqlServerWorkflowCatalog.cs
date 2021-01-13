using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.SqlServer
{
	public class SqlServerWorkflowCatalog : IWorkflowCatalog
	{
		private readonly IDbContext _dbContext;

		public SqlServerWorkflowCatalog(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<WorkflowElem> LoadBodyAsync(String id)
		{
			return _dbContext.LoadAsync<WorkflowElem>(null, "[A2v10.Workflow].[Catalog.Load]", new { Id = id });
		}

		public Task<WorkflowThumbElem> LoadThumbAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task SaveAsync(IWorkflowDescriptor workflow)
		{
			var eo = new ExpandoObject();
			eo.Set("Id", workflow.Id);
			eo.Set("Body", workflow.Body);
			eo.Set("Format", workflow.Format);
			eo.Set("ThumbFormat", workflow.ThumbFormat);
			return _dbContext.ExecuteExpandoAsync(null, "[A2v10.Workflow].[Catalog.Save]", eo);
		}
	}
}
