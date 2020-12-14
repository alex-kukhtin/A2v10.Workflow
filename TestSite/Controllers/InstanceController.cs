using A2v10.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSite.Controllers
{
	public class InstanceController : Controller
	{
		private readonly IDbContext _dbContext;

		public InstanceController(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IActionResult> Open(Guid id)
		{
			var prms = new ExpandoObject();
			prms.TryAdd("Id", id);
			var model = await _dbContext.LoadModelAsync(null, "[A2v10.Workflow].[Instance.Load]", prms);
			return View(model.GetDynamic()["Instance"]);
		}
	}
}
