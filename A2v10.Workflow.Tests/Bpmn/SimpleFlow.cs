
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
						Outgouing = new List<String>() { "script->end" }
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

			var tracker = new ConsoleTracker();
			var wfe = new WorkflowEngine(new InMemoryInstanceStorage(), tracker);
			await wfe.StartAsync(process, null);
		}
	}
}
