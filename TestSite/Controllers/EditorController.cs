using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSite.Controllers
{
	public class EditorController : Controller
	{
		public IActionResult Index(String id = null)
		{
			return View();
		}
	}
}
