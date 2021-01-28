﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Bpmn;
using A2v10.Workflow.Serialization;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests.Serialization
{
	[TestClass]
	[TestCategory("Serialization.Workflow")]
	public class SerializeWorkflow
	{
		[TestMethod]
		public async Task SimpleBpmnWorkflow()
		{
			var p = new Process()
			{
				Id = "process",
				IsExecutable = true,
				Children = new List<BaseElement>()
				{
					new StartEvent()
					{
						Id = "start",
						Children = new List<BaseElement>() { new Outgoing() {Text = "start->end" } }
					},
					new EndEvent()
					{
						Id = "end",
						Children = new List<BaseElement>() { new Incoming() { Text = "script->end"} }
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

			var sp = TestEngine.ServiceProvider();
			var wfs = sp.GetService<IWorkflowStorage>();
			var wfc = sp.GetService<IWorkflowCatalog>();

			await wfc.SaveAsync(new WorkflowDescriptor()
			{
				Id = "test1",
				Body = json,
				Format = "json"
			});
			var ident = await wfs.PublishAsync(wfc, "test1");

			var wfe = sp.GetService<IWorkflowEngine>();
			var inst = await wfe.CreateAsync(ident);
			await wfe.RunAsync(inst.Id);
		}

		[TestMethod]
		public async Task SimpleSequenceWorkflow()
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

			var sp = TestEngine.ServiceProvider();
			var wfs = sp.GetService<IWorkflowStorage>();
			var wfc = sp.GetService<IWorkflowCatalog>();

			await wfc.SaveAsync(new WorkflowDescriptor()
			{
				Id = "xxx",
				Body = "123",
				Format = "json"
			});

			// check for empty storage
			await wfs.PublishAsync(wfc, "xxx");

			await wfc.SaveAsync(new WorkflowDescriptor()
			{
				Id = "test2",
				Body = json,
				Format = "json"
			});

			var ident = await wfs.PublishAsync(wfc, "test2");
			Assert.AreEqual(1, ident.Version);

			ident = await wfs.PublishAsync(wfc, "test2");

			Assert.AreEqual(2, ident.Version);

			var wfe = sp.GetService<IWorkflowEngine>();
			var inst = await wfe.CreateAsync(ident);
			inst = await wfe.RunAsync(inst.Id, new { x = 5 });

			Assert.AreEqual(10, inst.Result.Get<Int32>("x"));

			inst = await wfe.ResumeAsync(inst.Id, "Bookmark1");
			Assert.AreEqual(15, inst.Result.Get<Int32>("x"));
		}


		[TestMethod]
		public async Task SimpleFlowchartWorkflow()
		{
			var fc = new Flowchart()
			{
				Id = "Ref0",
				Variables = new List<IVariable>
				{
					new Variable() {Name = "X", Dir = VariableDirection.InOut, Type=VariableType.Number},
				},
				Nodes = new List<FlowNode>()
				{
					new FlowStart() {Id="Ref1", Next="Ref2"},
					new FlowDecision() {Id="Ref2", Condition="X > 0", Then = "Ref3"},
					new FlowActivity() {Id = "Ref3", Next="Ref2",
						Activity = new Code() {Id="Ref4", Script="X -= 1" }
					}
				}
			};

			var ser = new Serializer();
			var json = ser.SerializeActitity(fc, "json");

			var sp = TestEngine.ServiceProvider();
			var wfs = sp.GetService<IWorkflowStorage>();
			var wfc = sp.GetService<IWorkflowCatalog>();

			await wfc.SaveAsync(new WorkflowDescriptor()
			{
				Id = "fchart1",
				Body = json,
				Format = "json"
			});

			var ident = await wfs.PublishAsync(wfc, "fchart1");

			Assert.AreEqual(1, ident.Version);

			ident = await wfs.PublishAsync(wfc, "fchart1");

			Assert.AreEqual(2, ident.Version);

			var wfe = sp.GetService<IWorkflowEngine>();
			var inst = await wfe.CreateAsync(ident);
			inst = await wfe.RunAsync(inst.Id, new { X = 5 });
			Assert.AreEqual(0, inst.Result.Get<Int32>("x"));
		}
	}
}
