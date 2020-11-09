
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;
using System;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Simple.Flow")]
	public class Script
	{
		[TestMethod]
		public async Task Sequence()
		{
			var root = new Sequence()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.In, Type=VariableType.Number},
					new Variable() {Name = "R", Dir = VariableDirection.Out, Type=VariableType.Number}
				},
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="X = X + 1"},
					new Code() {Ref="Ref2", Script="X = X + 1"},
					new Code() {Ref="Ref3", Script="R = X"},
				}
			};

			var wf = Workflow.Create(root, new { X = 5 });
			var result = await wf.RunAsync();
			Assert.AreEqual(7, result.Get<Int32>("R"));

			wf = Workflow.Create(root, new { X = 5 });
			result = await wf.RunAsync();
			Assert.AreEqual(7, result.Get<Int32>("R"));

			var wf2 = Workflow.Create(root, new { X = 11 });
			var result2 = await wf2.RunAsync();
			Assert.AreEqual(13, result2.Get<Int32>("R"));
		}

		[TestMethod]
		public async Task Parallel()
		{
			var root = new Parallel()
			{
				Ref = "Ref0",
				Branches = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="console.log('ref1')"},
					new Code() {Ref="Ref2", Script="console.log('ref2')"},
					new Code() {Ref="Ref3", Script="console.log('ref3')"},
				}
			};

			var wf = Workflow.Create(root);
			var result = await wf.RunAsync();
		}
	}
}
