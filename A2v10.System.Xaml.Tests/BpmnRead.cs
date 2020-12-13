
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.System.Xaml.Tests.Mock;
using System.IO;
using A2v10.Workflow.Bpmn;

namespace A2v10.System.Xaml.Tests
{
	[TestClass]
	[TestCategory("Xaml.Read.Bpmn")]
	public class BpmnRead
	{
		[TestMethod]
		public void SimpleFlow()
		{

			var xaml = File.ReadAllText("..\\..\\..\\TestFiles\\simple.bpmn");

			var obj = XamlServices.Parse(xaml);

			Assert.AreEqual(typeof(Definitions), obj.GetType());
			var defs = obj as Definitions;
			Assert.AreEqual("http://bpmn.io/bpmn", defs.TargetNamespace);
			Assert.AreEqual("sid-38422fae-e03e-43a3-bef4-bd33b32041b2", defs.Id);

			Assert.IsNotNull(defs.Children[0]);


			var proc = defs.Children[0] as Process;
			Assert.AreEqual("Process_1", proc.Id);
			Assert.AreEqual(false, proc.IsExecutable);

			Assert.AreEqual(1, proc.Elements.Count);

			var sev = proc.Elements[0] as StartEvent;

			Assert.AreEqual("StartEvent_1y45yut", sev.Id);
			Assert.AreEqual("hunger noticed", sev.Name);
			Assert.AreEqual(1, sev.Children.Count);

			var og = sev.Children[0] as Outgoing;
			Assert.AreEqual("SequenceFlow_0h21x7r", og.Text);
		}

		[TestMethod]
		public void ParallelBmpn()
		{
			var xaml = File.ReadAllText("..\\..\\..\\TestFiles\\parallel.bpmn");

			var obj = XamlServices.Parse(xaml);

			Assert.AreEqual(typeof(Definitions), obj.GetType());
			var defs = obj as Definitions;
			var proc = defs.Process;
			Assert.AreEqual("Process_1", proc.Id);
			Assert.AreEqual("Parallel Process", proc.Name);
			Assert.AreEqual(true, proc.IsExecutable);

			var se = proc.Elem<StartEvent>();
			Assert.IsNotNull(se);
			Assert.AreEqual("StartEvent_1", se.Id);

			var ee = proc.Elem<EndEvent>();
			Assert.IsNotNull(ee);
			Assert.AreEqual("EndEvent_0zk83sa", ee.Id);
		}
	}
}
