using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestSite.Models;

using A2v10.Data.Interfaces;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.SqlServer;
using System.Dynamic;
using A2v10.Data.Extensions;

namespace TestSite.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IDbContext _dbContext;
		private readonly IWorkflowCatalog _workflowCatalog;

		public HomeController(ILogger<HomeController> logger, IDbContext dbContext, IWorkflowCatalog workflowCatalog)
		{
			_logger = logger;
			_dbContext = dbContext;
			_workflowCatalog = workflowCatalog;
		}

		public async Task<IActionResult> Index()
		{
			IDataModel dm;
			switch (_workflowCatalog)
			{
				case FilesystemCatalog fsc:
					{
						dynamic o = new ExpandoObject();
						o.Workflows = fsc.GetWorkflowsList().Select(wf =>
						{
							dynamic itm = new ExpandoObject();
							itm.Id = wf.name;
							itm.Format = "text/xml";
							itm.DateCreated = wf.createdTime;
							return itm;
						}).ToList();
						dm = await _dbContext.SaveModelAsync(null, "a2wfui.[WorkflowList.Update]", o);
						break;
					}
				case SqlServerWorkflowCatalog:
					dm = await _dbContext.LoadModelAsync(null, "a2wfui.[Workflows.Index]");
					break;
				default:
					throw new Exception("Unrcognized Catalog type");
			}
			return View(dm.GetDynamic()["Workflows"]);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
