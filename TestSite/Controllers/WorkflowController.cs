using A2v10.Workflow;
using A2v10.Workflow.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSite.Controllers
{
	public class WorkflowController : Controller
	{
		private readonly IWorkflowEngine _engine;
		private readonly IWorkflowCatalog _catalog;

		public WorkflowController(IWorkflowEngine engine, IWorkflowCatalog catalog)
		{
			_engine = engine;
			_catalog = catalog;
		}

		[HttpPost]
		public async Task<IActionResult> Publish(String id, IFormFile uploadedFile)
		{
			if (id == null || uploadedFile == null)
				return View("Error");
			String format = uploadedFile.ContentType switch {
				"text/xml" => "xaml",
				"application/octet-stream" => "xaml",
				_ => throw new InvalidOperationException($"invalid format '{uploadedFile.ContentType}'")
			};
			using var ms = new MemoryStream();
			uploadedFile.CopyTo(ms);
			var text = Encoding.UTF8.GetString(ms.ToArray());
			await _catalog.SaveAsync(new WorkflowDescriptor()
			{
				Id = id,
				Body = text,
				Format = format
			});
			return Redirect("/");
		}

		[HttpPost]
		public async Task<IActionResult> Run(String id, Int32 version)
		{
			var identity = new Identity() { Id = id, Version = version };
			await _engine.StartAsync(identity); //, new { X = 5 });
			return Redirect("/");
		}
	}
}
