using A2v10.Data.Interfaces;
using A2v10.Workflow;
using A2v10.Workflow.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace TestSite.Controllers
{
	public class EditorController : Controller
	{
		private readonly IWorkflowStorage _workflowStorage;
		private readonly IWorkflowCatalog _workflowCatalog;
		private readonly IWebHostEnvironment _host;
		public EditorController(IWorkflowStorage workflowStorage, IWorkflowCatalog workflowCatalog, IWebHostEnvironment host)
		{
			_workflowStorage = workflowStorage;
			_workflowCatalog = workflowCatalog;
			_host = host;
		}
		public IActionResult Index(String id = null)
		{
			ViewBag.Id = id;
			return View();
		}

		public async Task<IActionResult> Get(String id = null)
		{
			String source;
			if (!String.IsNullOrEmpty(id))
			{
				var elem = await _workflowCatalog.LoadBodyAsync(id);
				source = elem.Body;
			} 
			else
			{
				var fi = _host.WebRootFileProvider.GetFileInfo("workflows/default.bpmn");
				source = System.IO.File.ReadAllText(fi.PhysicalPath);
			}
			return Content(source);
		}

		public async Task<IActionResult> Set(String id, String content)
		{
			await _workflowCatalog.SaveAsync(new WorkflowDescriptor()
			{
				Id = id,
				Body = content,
				Format = "text/xml"
			});
			return new EmptyResult();
		}

		public IActionResult Viewer(String id)
		{
			ViewBag.Id = id;
			return View();
		}
	}
}
