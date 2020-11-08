
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Simple.Flow")]
	public class SimpleFlow
	{
		[TestMethod]
		public async Task Sequence()
		{
			var root = new Sequence()
			{
				Ref = "Ref0",
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="print ref1"},
					new Code() {Ref="Ref2", Script="print ref2"},
					new Code() {Ref="Ref3", Script="print ref3"},
				}
			};

			var wf = new Workflow(root);
			await wf.RunAsync();
		}

		[TestMethod]
		public async Task Parallel()
		{
			var root = new Parallel()
			{
				Ref = "Ref0",
				Branches = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="print ref1"},
					new Code() {Ref="Ref2", Script="print ref2"},
					new Code() {Ref="Ref3", Script="print ref3"},
				}
			};

			var wf = new Workflow(root);
			await wf.RunAsync();
		}
	}
}
