
using A2v10.Workflow.Bpmn;
using A2v10.Workflow.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("Bmpn.Simple")]
	public class BpmnSimle
	{
		[TestMethod]
		public async Task Sequence()
		{
			var process = new Process()
			{
				Id = "process",
				IsExecutable = true,
				Elements = new List<BpmnElement>()
				{
					new StartEvent()
					{
						Id = "start",
						Outgoing = new List<String>() { "start->script" }
					},
					new ScriptTask()
					{
						Id = "script",
						Incoming  = new List<String>() { "start->script" },
						Outgoing = new List<String>() { "script->end" }
					},
					new EndEvent()
					{
						Id = "end",
						Incoming = new List<String>() {"script->end"}
					},
					new SequenceFlow()
					{
						Id = "start->script",
						SourceRef = "start",
						TargetRef = "script"
					},
					new SequenceFlow()
					{
						Id = "script->end",
						SourceRef = "script",
						TargetRef = "end"
					}
				}
			};

			var wfe = TestEngine.CreateInMemoryEngine();
			await wfe.StartAsync(process, null);
		}


		[TestMethod]
		public async Task ParallelGateway()
		{
			var process = new Process()
			{
				Id = "process",
				IsExecutable = true,
				Elements = new List<BpmnElement>()
				{
					new StartEvent()
					{
						Id = "start",
						Outgoing = new List<String>() { "start->gate1" }
					},
					new ParallelGateway()
					{
						Id = "gate1",
						Incoming  = new List<String>() { "start->gate1" },
						Outgoing = new List<String>() { "gate1->task1", "gate1->task2" }
					},
					new ScriptTask()
					{
						Id = "task1",
						Incoming  = new List<String>() { "gate1->task1" },
						Outgoing = new List<String>() { "task1->gate2" }
					},
					new ScriptTask()
					{
						Id = "task2",
						Incoming  = new List<String>() { "gate1->task2" },
						Outgoing = new List<String>() { "task2->gate2" }
					},
					new ParallelGateway()
					{
						Id = "gate2",
						Incoming  = new List<String>() { "task1->gate2", "task2->gate2" },
						Outgoing = new List<String>() { "gate2->end", }
					},
					new EndEvent()
					{
						Id = "end",
						Incoming = new List<String>() {"script->end"}
					},
					new SequenceFlow() {Id = "start->gate1", SourceRef = "start", TargetRef = "gate1"},
					new SequenceFlow() {Id = "gate1->task1", SourceRef = "gate1", TargetRef = "task1"},
					new SequenceFlow() {Id = "gate1->task2", SourceRef = "gate1", TargetRef = "task2"},
					new SequenceFlow() {Id = "task1->gate2", SourceRef = "task1", TargetRef = "gate2"},
					new SequenceFlow() {Id = "task2->gate2", SourceRef = "task2", TargetRef = "gate2"},
					new SequenceFlow() {Id = "gate2->end",   SourceRef = "gate2", TargetRef = "end"}
				}
			};

			var wfe = TestEngine.CreateInMemoryEngine();
			await wfe.StartAsync(process, null);
		}
	}
}
