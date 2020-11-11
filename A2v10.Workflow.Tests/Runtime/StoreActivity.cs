using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Storage;
using A2v10.Workflow.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests.Runtime
{
	[TestClass]
	[TestCategory("Runtime.Store")]
	public class StoreActivity
	{
		public class ActivityWrapper
		{
			public IActivity Root { get; set; }
		}

		[TestMethod]
		public async Task StoreSequence()
		{
			IActivity root = new Sequence()
			{
				Ref = "Ref0",
				Variables = new List<IVariable>()
				{
					new Variable() {Name = "x", Dir= VariableDirection.InOut}
				},
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="x += 5"},
					new Wait() {Ref="Ref2", Bookmark="Bookmark1"},
					new Code() {Ref="Ref3", Script="x += 5"},
				}
			};

			var wrap = new ActivityWrapper() { Root = root};

			var jsonSettings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto,
				Converters = new List<JsonConverter>() { new StringEnumConverter(), new DoubleConverter() }
			};

			var rootJS = JsonConvert.SerializeObject(wrap, jsonSettings);
			//var act = JsonConvert.DeserializeObject<ActivityWrapper>(rootJS, jsonSettings);

			Console.WriteLine(rootJS);

			var instStorage = new InMemoryInstanceStorage();
			var tracker = new ConsoleTracker();

			var wfe = new WorkflowEngine(instStorage, tracker);
			var inst = await wfe.StartAsync(root, new { x = 5 });
			Assert.AreEqual(10, inst.Result.Get<Int32>("x"));

			var resume = new WorkflowEngine(instStorage, tracker);
			var resInst = await resume.ResumeAsync(inst.Id, "Bookmark1");
			Assert.AreEqual(15, resInst.Result.Get<Int32>("x"));
		}
	}
}
