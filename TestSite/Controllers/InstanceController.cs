using A2v10.Data;
using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;
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
		private readonly IWorkflowEngine _engine;

		public InstanceController(IDbContext dbContext, IWorkflowEngine engine)
		{
			_dbContext = dbContext;
			_engine = engine;
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


		public async Task<IActionResult> Resume(Guid id)
		{
			var prms = new ExpandoObject();
			prms.TryAdd("Id", id);
			var model = await _dbContext.LoadModelAsync(null, "a2wfui.[Instance.Load]", prms);

			var stateJson = model.Eval<String>("Instance.State");
			var stateObj = JsonConvert.DeserializeObject<ExpandoObject>(stateJson ?? "{}");
			var bk = stateObj.Eval<ExpandoObject>("Bookmarks") as IDictionary<String, Object>;
			foreach (var (k, _) in bk)
			{
				var exp = JsonConvert.DeserializeObject<ExpandoObject>("{Answer:'Success'}");
				await _engine.ResumeAsync(id, k, exp);
			}
			return LocalRedirect("/instance/index");
		}
	}
}
