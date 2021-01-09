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
		public EditorController(IWorkflowStorage workflowStorage)
		{
			_workflowStorage = workflowStorage;
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
				source = await _workflowStorage.LoadSourceAsync(new Identity() { Id = id });
			return Content(source);
		}
	}
}
