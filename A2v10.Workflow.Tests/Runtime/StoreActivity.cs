using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests.Runtime
{
	[TestClass]
	[TestCategory("Runtime.Store")]
	public class StoreActivity
	{
		[TestMethod]
		public async Task StoreSequence()
		{
			var root = new Sequence()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>()
				{
					new Variable() {Name = "x", Dir= VariableDirection.InOut}
				},
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="x += 5"},
					new Wait() {Ref="Ref2", Bookmark="Bookmark1"},
					new Code() {Ref="Ref3", Script="x += 5"},
				}
			};

			var rootJS = JsonConvert.SerializeObject(root, Formatting.Indented);
			Console.WriteLine(rootJS);

			IWorkflow wf = Workflow.Create(root, new { x = 5 });
			var result = await wf.RunAsync();
			Assert.AreEqual(10, result.Get<Int32>("x"));

			result = await wf.ResumeAsync("Bookmark1", null);
			Assert.AreEqual(15, result.Get<Int32>("x"));
		}
	}
}
