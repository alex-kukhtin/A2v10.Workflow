using A2v10.Data.Interfaces;
using A2v10.Workflow;
using A2v10.Workflow.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSite.Controllers
{
	public class EditorController : Controller
	{
		private readonly IWorkflowStorage _workflowStorage;
		private readonly IWorkflowCatalog _workflowCatalog;
		public EditorController(IWorkflowStorage workflowStorage, IWorkflowCatalog workflowCatalog)
		{
			_workflowStorage = workflowStorage;
			_workflowCatalog = workflowCatalog;
		}
		public IActionResult Index(String id = null)
		{
			ViewBag.Id = id;
			return View();
		}

		public async Task<IActionResult> Get(String id = null)
		{
			String source = null;
			if (!String.IsNullOrEmpty(id))
			{
				var elem = await _workflowCatalog.LoadBodyAsync(id);
				source = elem.Body;
			}
			return Content(source);
		}
	}
}
