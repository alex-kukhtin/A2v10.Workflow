using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Bpmn;
using A2v10.Workflow.Serialization;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests.Serialization
{
	[TestClass]
	[TestCategory("Serialization.Instance")]
	public class SerializeInstance
	{
		[TestMethod]
		public void SimpleProcessJson()
		{
			var p = new Process()
			{
				Id = "process",
				IsExecutable = true,
				Elements = new List<BaseElement>()
				{
					new StartEvent()
					{
						Id = "start",
						Children = new List<BaseElement>() { new Outgoing() {Text = "start->script" } }
					},
					new EndEvent()
					{
						Id = "end",
						Children = new List<BaseElement>() {new Incoming() {Text = "script->end"} }
					},
					new SequenceFlow()
					{
						Id = "start->end",
						SourceRef = "start",
						TargetRef = "end"
					}
				}
			};

			var s = new Serializer();
			var json = s.SerializeActitity(p, "json");

			var r = s.DeserializeActitity(json, "json") as Process;

			Assert.AreEqual(r.Id, p.Id);
			Assert.AreEqual(r.Elements.Count, p.Elements.Count);

			var pEvent = p.FindElement<Event>("start");
			var rEvent = r.FindElement<Event>("start");
			Assert.AreEqual(pEvent.Id, rEvent.Id);
			Assert.AreEqual(pEvent.IsStart, rEvent.IsStart);

			Console.WriteLine(json);
		}

		[TestMethod]
		public void SimpleSequenceJson()
		{
			Sequence s = new Sequence()
			{
				Id = "Ref0",
				Variables = new List<IVariable>()
				{
					new Variable() {Name = "x", Dir= VariableDirection.InOut}
				},
				Activities = new List<IActivity>()
				{
					new Code() {Id="Ref1", Script="x += 5"},
					new Wait() {Id="Ref2", Bookmark="Bookmark1"},
					new Code() {Id="Ref3", Script="x += 5"},
				}
			};

			var ser = new Serializer();
			var json = ser.SerializeActitity(s, "json");

			var r = ser.DeserializeActitity(json, "json") as Sequence;

			Assert.AreEqual(r.Id, s.Id);
			Assert.AreEqual(r.Variables.Count, s.Variables.Count);
			Assert.AreEqual(r.Activities.Count, s.Activities.Count);

			Assert.AreEqual(r.Activities[0].Id, s.Activities[0].Id);
			Assert.AreEqual(r.Activities[1].Id, s.Activities[1].Id);
			Assert.AreEqual(r.Activities[0].GetType(), typeof(Code));
			Assert.AreEqual(r.Activities[1].GetType(), typeof(Wait));
			Assert.AreEqual(r.Activities[2].GetType(), typeof(Code));
		}
	}
}
