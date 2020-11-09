
using System;
using System.Collections.Generic;
using System.Dynamic;

using Jint;
using Jint.Native;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class ScriptEngine
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Engine _engine;
		private readonly ExpandoObject _scriptData;
		private readonly IActivity _root;
		
		private IDictionary<String, Object> ScriptData => _scriptData;

		public ScriptEngine(IServiceProvider serviceProvider, IActivity root, String script, Object args = null)
		{
			_serviceProvider = serviceProvider;
			_root = root;
			_engine = new Engine(EngineOptions);
			_engine.AddNativeObjects();

			Console.WriteLine(script);
			var func = _engine.Execute(script).GetCompletionValue();
			_scriptData = func.Invoke().ToObject() as ExpandoObject;
			if (args != null)
				SetArguments(args);
		}

		private static void EngineOptions(Options opts)
		{
			opts.Strict(true);
		}

		void SetArguments(Object args)
		{
			var func = GetFunc(_root.Ref, "Arguments");
			if (func == null)
				throw new WorkflowExecption($"Script element {_root.Ref}.Arguments not found");
			func(JsValue.Undefined, new JsValue[] { JsValue.FromObject(_engine, args) });
		}

		public void Restore(String refer, Object args)
		{
			var func = GetFunc(refer, "Restore");
			if (func == null)
				throw new WorkflowExecption($"Script element {refer}.Restore not found");
			func(JsValue.Undefined, new JsValue[] { JsValue.FromObject(_engine, args) });
		}

		public ExpandoObject GetResult()
		{
			var func = GetFunc(_root.Ref, "Result");
			if (func == null)
				return null;
			return func(JsValue.Undefined, null).ToObject() as ExpandoObject;
		}

		public T Evaluate<T>(String refer, String name)
		{
			var func = GetFunc(refer, name);
			if (func == null)
				throw new WorkflowExecption($"Script element {refer}.{name} not found");
			var obj = func(JsValue.Undefined, null).ToObject();
			if (obj is T objT)
				return objT;
			return (T) Convert.ChangeType(obj, typeof(T));
		}

		private Func<JsValue, JsValue[], JsValue> GetFunc(String refer, String name)
		{
			if (ScriptData.TryGetValue(refer, out Object activityData))
			{
				if (activityData is IDictionary<String, Object> expData)
				{
					if (expData.TryGetValue(name, out Object objVal))
						return (Func<JsValue, JsValue[], JsValue>) objVal;
				}
			}
			return null;
		}

		public void Execute(String refer, String name)
		{
			var func = GetFunc(refer, name);
			if (func == null)
				throw new WorkflowExecption($"Script element {refer}.{name} not found");
			func(JsValue.Undefined, null);
		}
	}
}
