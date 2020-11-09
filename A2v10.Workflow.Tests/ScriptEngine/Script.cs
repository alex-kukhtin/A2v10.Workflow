
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Workflow.Interfaces;
using System.Threading.Tasks;
using System;
using Jint;
using System.Runtime.InteropServices.ComTypes;
using Jint.Native;

namespace A2v10.Workflow.Tests
{
	[TestClass]
	[TestCategory("ScriptEngine")]
	public class TestScript
	{

		const String program = @"
() => {
	var _fmap_ = {};

	_fmap_['Ref0'] = {
		Script: () => 7
	};

	return _fmap_;
};
";

		[TestMethod]
		public void ScriptEngine()
		{
			var eng = new Engine(opts => 
			{
				opts.Strict(true);
			});


			var obj = eng.Execute(program).GetCompletionValue().ToObject();
			var func = obj as Func<JsValue, JsValue[], JsValue>; 

			var val = func.Invoke(JsValue.Undefined, null).ToObject();

			if (val is IDictionary<String, Object> dict)
			{
				var x = dict["Ref0"] as IDictionary<String, Object>;
				var res = JsValue.FromObject(eng, x["Script"]).Invoke();
				Assert.AreEqual("7", res.ToString());
			} else
			{
				Assert.Fail("Ref0 not found");
			}

		}
	}
}
