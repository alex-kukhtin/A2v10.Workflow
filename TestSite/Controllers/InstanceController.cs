using A2v10.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TestSite.Models;

namespace TestSite.Controllers
{
	public class InstanceController : Controller
	{
		private readonly IDbContext _dbContext;

		public InstanceController(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}


		public async Task<IActionResult> Index()
		{
			var dm = await _dbContext.LoadModelAsync(null, "a2wfui.[Instances.Index]");
			return View(dm.GetDynamic()["Instances"]);
		}

		public async Task<IActionResult> Open(Guid id)
		{
			var prms = new ExpandoObject();
			prms.TryAdd("Id", id);
			var model = await _dbContext.LoadModelAsync(null, "a2wfui.[Instance.Load]", prms);

			var stateJson = model.Eval<String>("Instance.State");

			var stateObj = JsonConvert.DeserializeObject<ExpandoObject>(stateJson ?? "{}");
			var jsonResult = JsonConvert.SerializeObject(stateObj, Formatting.Indented);


			var instOpenModel = new InstanceOpenModel()
			{
				Id = id.ToString(),
				State = jsonResult,
				Instance = model.Eval<ExpandoObject>("Instance")
			};
			return View(instOpenModel);
		}
	}
}
