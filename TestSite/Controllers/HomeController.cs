using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestSite.Models;

using A2v10.Data.Interfaces;

namespace TestSite.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IDbContext _dbContext;

		public HomeController(ILogger<HomeController> logger, IDbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
		}

		public async Task<IActionResult> Index()
		{
			// workflows
			var dm = await _dbContext.LoadModelAsync(null, $"a2wfui.[Workflows.Index]");
			return View(dm.GetDynamic()["Workflows"]);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
