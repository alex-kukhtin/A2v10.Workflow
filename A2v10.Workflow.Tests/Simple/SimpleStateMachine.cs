
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;
using System;
using A2v10.Workflow.Storage;
using System.Diagnostics;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Simple.StateMachine")]
	public class SimpleStateMachine
	{
		[TestMethod]
		public async Task Simple()
		{
			var root = new StateMachine()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.InOut, Type=VariableType.Number},
				},
				States = new List<StateBase>()
				{
					new StartState() {Ref="Ref1", Next="Ref2"},
					new State() {Ref="Ref2", Next="Ref3",
						Entry = new Code() {Ref="Ref21", Script = "X +=1"},
					},
					new FinalState {Ref="Ref3"}
				}
			};

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(6, result.Get<Int32>("X"));
		}

		[TestMethod]
		public async Task Counter()
		{
			var root = new StateMachine()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.InOut, Type=VariableType.Number},
				},
				States = new List<StateBase>()
				{
					new StartState() {Ref="Ref1", Next="Ref2"},
					new State() {Ref="Ref2", Next="Ref3",
						Entry = new Code() {Ref="Ref21", Script = "X -=1"},
						Transitions = new List<Transition>() {
							new Transition() { Ref = "Ref211", Condition="X > 0", Destination="Ref2" }
						}
					},
					new FinalState {Ref="Ref3"}
				}
			};

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			var inst = await wfe.StartAsync(root, new { X = 5 });
			var result = inst.Result;
			Assert.AreEqual(0, result.Get<Int32>("X"));
		}
	}
}
