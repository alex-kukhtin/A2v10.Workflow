
using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace A2v10.Workflow.SqlServer.Activities
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class ExecuteSql : Activity
	{
		private readonly IDbContext _dbContext;

		public String DataSource { get; set; }
		public String Procedure { get; set; }

		public ExecuteSql(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public override async ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var prms = new ExpandoObject();
			var result = await _dbContext.ExecuteAndLoadAsync<ExpandoObject, ExpandoObject>(DataSource, Procedure, prms);
			// TODO result
			if (onComplete != null)
				await onComplete(context, this);
		}
	}
}
