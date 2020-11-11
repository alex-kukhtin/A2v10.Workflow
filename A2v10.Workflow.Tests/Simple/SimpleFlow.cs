
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;
using System;
using A2v10.Workflow.Storage;

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

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(7, result.Get<Int32>("R"));

			inst = await wfe.StartAsync(root, new { X = 3 });
			result = inst.Result;
			Assert.AreEqual(5, result.Get<Int32>("R"));

			inst = await wfe.StartAsync(root, new { X = 11 });
			Assert.AreEqual(13, inst.Result.Get<Int32>("R"));
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

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			await wfe.StartAsync(root);
		}

		[TestMethod]
		public async Task If()
		{
			var root = new Sequence()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.In, Type=VariableType.Number},
					new Variable() {Name = "R", Dir = VariableDirection.Out, Type=VariableType.String}
				},
				Activities = new List<IActivity>()
				{
					new If() {
						Ref="Ref1",
						Condition="X > 5",
						Then = new Code() {Ref="Ref2", Script = "R = 'X > 5'"},
						Else = new Code() {Ref="Ref3", Script = "R = 'X <= 5'"}
					}
				}
			};

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 4});
			Assert.AreEqual("X <= 5", inst.Result.Get<String>("R"));

			inst = await wfe.StartAsync(root, new { X = 23 });
			Assert.AreEqual("X > 5", inst.Result.Get<String>("R"));
		}


		[TestMethod]
		public async Task NestedContext()
		{
			var root = new Sequence()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.In, Type=VariableType.Number},
					new Variable() {Name = "R", Dir = VariableDirection.Out, Type=VariableType.Number},
				},
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="X = X + 1"},
					new Code() {Ref="Ref2", Script="X = X + 1"},
					new Sequence() { Ref="Ref3",
						Activities = new List<IActivity>()
						{
							new Code() {Ref="Ref4", Script="X = X + 1"},
							new Code() {Ref="Ref5", Script="X = X + 1"},
						}
					},
					new Code() {Ref="Ref7", Script="R = X"},
				}
			};

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(9, result.Get<Int32>("R"));
		}
	}
}
