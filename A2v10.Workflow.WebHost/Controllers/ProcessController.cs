using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2v10.Workflow.WebApi;
using A2v10.Workflow.Interfaces.Api;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.WebHost.Controllers
{
	[Route("api/process")]
	[ApiController]
	public class ProcessController : Controller
	{
		private readonly IWorkflowApi api;

		public ProcessController(IWorkflowApi api)
		{
			this.api = api;
		}

		[HttpPost]
		[Route("start")]
		public async ValueTask<IActionResult> Start(StartProcessRequest req)
		{
			try
			{
				var r = await api.StartAsync(req);
				return Ok(new StartProcessResponse(r, false));
			}
			catch (ApiException e)
			{
				return StatusCode(422, new ErrorResponse(e));
			}
		}

		[HttpPost]
		[Route("resume")]
		public async ValueTask<IActionResult> Resume(ResumeProcessRequest req)
		{
			try
			{
				var r = await api.ResumeAsync(req);
				return Ok(new ResumeProcessResponse(r, false));
			}
			catch (ApiException e)
			{
				return StatusCode(422, new ErrorResponse(e));
			}
		}
	}
}
