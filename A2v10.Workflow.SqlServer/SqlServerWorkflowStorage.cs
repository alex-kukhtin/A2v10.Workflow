﻿using System;
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
		private readonly ISerializer _serializer;

		private const String Schema = "[A2v10.Workflow]";

		public SqlServerWorkflowStorage(IDbContext dbContext, ISerializer serializer)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}

		public async ValueTask<IWorkflow> LoadAsync(IIdentity identity)
		{
			var prms = new ExpandoObject();
			prms.TryAdd("Id", identity.Id);
			prms.TryAdd("Version", identity.Version);

			var eo = await _dbContext.ReadExpandoAsync(null, $"{Schema}.[Workflow.Load]", prms);

			return new Workflow()
			{
				Identity = new Identity()
				{
					Id = eo.Get<String>("WorkflowId"),
					Version = eo.Get<Int32>("Version")
				},
				Root = _serializer.DeserializeActitity(eo.Get<String>("Text"), eo.Get<String>("Format"))
			};
		}

		public async ValueTask<IIdentity> PublishAsync(String id, String text, String format)
		{
			var prms = new ExpandoObject();
			var d= prms as IDictionary<String, Object>;
			d.Add("Id", id);
			d.Add("Format", format);
			d.Add("Text", text);
			var res = await _dbContext.ReadExpandoAsync(null, $"{Schema}.[Workflow.Publish]", prms);

			return new Identity()
			{
				Id = res.Get<String>("Id"),
				Version = res.Get<Int32>("Version")
			};
		}
	}
}