using A2v10.Workflow.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Bmpn.Full")]
	public class FullFlow
	{
		[TestMethod]
		public async Task Parallel()
		{
			var xaml = File.ReadAllText("..\\..\\..\\TestFiles\\parallel_1.bpmn");

			var sp = TestEngine.ServiceProvider();

			var wfs = sp.GetService<IWorkflowStorage>();
			var ident = await  wfs.PublishAsync("Parallel1", xaml, "xaml");

			var wfe = sp.GetService<IWorkflowEngine>();
			var inst = await wfe.StartAsync(ident, new { X = 5 });
			var res = inst.Result;

			Assert.AreEqual(12, res.Get<Int32>("X"));
		}

		[TestMethod]
		public async Task UserTask()
		{
			var xaml = File.ReadAllText("..\\..\\..\\TestFiles\\user_task_1.bpmn");

			var sp = TestEngine.ServiceProvider();

			var wfs = sp.GetService<IWorkflowStorage>();
			var ident = await wfs.PublishAsync("Wait1", xaml, "xaml");

			var wfe = sp.GetService<IWorkflowEngine>();
			var inst = await wfe.StartAsync(ident, new { X = 5 });
			var res = inst.Result;
			Assert.AreEqual(10, res.Get<Int32>("X"));

			inst = await wfe.ResumeAsync(inst.Id, "Activity_1rffhs5", new { V = 5 });
			res = inst.Result;
			Assert.AreEqual(15, res.Get<Int32>("X"));
			

		}
	}
}
