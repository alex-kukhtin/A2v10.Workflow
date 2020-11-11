
using System;
using System.Dynamic;
using System.Threading.Tasks;

using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.SqlServer.Activities
{
	public class ExecuteSql : Activity
	{
		private readonly IDbContext _dbContext;

		public String DataSource { get; set; }
		public String Procedure { get; set; }

		public ExecuteSql(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public override async ValueTask ExecuteAsync(IExecutionContext context, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			var prms = new ExpandoObject();
			await _dbContext.ExecuteAndLoadAsync<ExpandoObject, ExpandoObject>(DataSource, Procedure, prms);
			// TODO result
			if (onComplete != null)
				await onComplete(context, this);
		}
	}
}
