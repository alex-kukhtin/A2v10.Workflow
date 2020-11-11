
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;
using System;
using A2v10.Workflow.Storage;
using A2v10.Workflow.Activities;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Simple.Flowchart")]
	public class SimpleFlowchart
	{
		[TestMethod]
		public async Task Simple()
		{
			var root = new Flowchart()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.InOut, Type=VariableType.Number},
				},
				Nodes = new List<FlowNode>()
				{
					new FlowStart() {Ref="Ref1", Next="Ref2"},
					new FlowDecision() {Ref="Ref2", Condition="X > 0", Then = "Ref3"},
					new FlowActivity() {Ref = "Ref3", Next="Ref2",
						Activity = new Code() {Ref="Ref4", Script="X -= 1" }
					}
				}
			};

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(0, result.Get<Int32>("X"));

			inst = await wfe.StartAsync(root, new { X = 3 });
			result = inst.Result;
			Assert.AreEqual(0, result.Get<Int32>("X"));
		}

		[TestMethod]
		public async Task Bookmarks()
		{
			var root = new Flowchart()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.InOut, Type=VariableType.Number},
				},
				Nodes = new List<FlowNode>()
				{
					new FlowStart() {Ref="Ref1", Next="Ref2"},
					new FlowDecision() {Ref="Ref2", Condition="X > 0", Then = "Ref3"},
					new FlowActivity() {Ref = "Ref3", Next="Ref2",
						Activity = new Sequence() {
							Ref="Ref4", 
							Activities = new List<IActivity>() {
								new Wait() {Ref="Ref5", Bookmark="BM1" },
								new Code() {Ref="Ref6", Script="X -= 1" }
							}
						}
					}
				}
			};

			var storage = new InMemoryInstanceStorage();
			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(storage, tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(5, result.Get<Int32>("X"));
			var id = inst.Id;

			int x = result.Get<Int32>("X");
			while (x > 0)
			{
				var resume = new WorkflowEngine(storage, tracker);
				var resInst = await resume.ResumeAsync(inst.Id, "BM1");
				x = resInst.Result.Get<Int32>("X");
			}
			Assert.AreEqual(0, x);
		}
	}
}
