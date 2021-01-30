using A2v10.Workflow;
using A2v10.Workflow.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSite.Models;

namespace TestSite.Controllers
{
	public class WorkflowController : Controller
	{
		private readonly IWorkflowEngine _engine;
		private readonly IWorkflowCatalog _catalog;
		private readonly IWorkflowStorage _storage;

		public WorkflowController(IWorkflowEngine engine, IWorkflowCatalog catalog, IWorkflowStorage storage, ITracker tracker)
		{
			_engine = engine;
			_catalog = catalog;
			_storage = storage;
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
			var inst = await _engine.CreateAsync(identity); //, new { X = 5 });
			await _engine.RunAsync(inst.Id);
			return Redirect("/");
		}

		[HttpPost]
		public async Task<IActionResult> PublishCatalog(String id)
		{
			await _storage.PublishAsync(_catalog, id);
			return Redirect("/");
		}

		[HttpGet]
		public IActionResult Run2(String id)
		{
			var m = new Run2Model()
			{
				Id = id,
				Parameter = "{}"
			};
			return View(m);
		}

		[HttpPost]
		[ActionName("Run2")]
		public async Task<IActionResult> HttpPost(Run2Model model) 
		{
			var prms = JsonConvert.DeserializeObject<ExpandoObject>(model.Parameter);
			if (prms != null && (prms as IDictionary<String, Object>).Count == 0)
				prms = null;
			var inst = await _engine.CreateAsync(new Identity() { Id = model.Id });
			await _engine.RunAsync(inst.Id, prms);
			return Redirect("/");
		}
	}
}
