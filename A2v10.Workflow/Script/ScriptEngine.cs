﻿
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Tracker;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.Workflow
{
	public class ScriptEngine
	{
		private readonly Engine _engine;
		private readonly ExpandoObject _scriptData;
		private readonly IActivity _root;
		private readonly IServiceProvider _serviceProvider;
		private readonly ITracker _tracker;
		private readonly IDeferredTarget _deferredTarget;

		private IDictionary<String, Object> ScriptData => _scriptData;

		public ScriptEngine(IServiceProvider serviceProvider, ITracker tracker, IActivity root, String script, Object args = null)
		{
			_root = root;
			_serviceProvider = serviceProvider;
			_tracker = tracker;
			_engine = new Engine(EngineOptions);
			_deferredTarget = _serviceProvider.GetService<IDeferredTarget>();


			var _nativeObjects = _serviceProvider.GetService<IScriptNativeObjectProvider>();
			_engine.AddNativeObjects(_nativeObjects);

			//Console.WriteLine(script);
			var func = _engine.Execute(script).GetCompletionValue();
			_scriptData = func.Invoke().ToObject() as ExpandoObject;
			if (args != null)
				SetArguments(args);
		}

		private void EngineOptions(Options opts)
		{
			opts.Strict(true);
			opts.SetWrapObjectHandler((e, o) =>
			{
				if (o is IInjectable injectable)
					injectable.Inject(_serviceProvider);
				return new ObjectWrapper(e, o);
			});
		}

		void SetArguments(Object args)
		{
			var func = GetFunc(_root.Id, "Arguments");
			if (func == null)
				throw new WorkflowException($"Script element {_root.Id}.Arguments not found");
			func(JsValue.Undefined, new JsValue[] { JsValue.FromObject(_engine, args) });
		}

		public void Restore(String refer, Object args)
		{
			var func = GetFunc(refer, "Restore");
			if (func == null)
				return;
			func(JsValue.Undefined, new JsValue[] { JsValue.FromObject(_engine, args) });
		}

		public ExpandoObject GetResult()
		{
			var func = GetFunc(_root.Id, "Result");
			if (func == null)
				return null;
			return func(JsValue.Undefined, null).ToObject() as ExpandoObject;
		}

		public T Evaluate<T>(String refer, String name)
		{
			_deferredTarget.Refer = refer;
			//_tracker.Track(new ScriptTrackRecord(ScriptTrackAction.Evaluate, refer, name));
			var func = GetFunc(refer, name);
			T res = default;
			if (func == null)
				res = default;
			else
			{
				var obj = func(JsValue.Undefined, null).ToObject();
				if (obj is T objT)
					res = objT;
				else
					res = (T)Convert.ChangeType(obj, typeof(T));
			}
			//_tracker.Track(new ScriptTrackRecord(ScriptTrackAction.EvaluateResult, refer, name, res));
			return res;
		}

		private Func<JsValue, JsValue[], JsValue> GetFunc(String refer, String name)
		{
			if (ScriptData.TryGetValue(refer, out Object activityData))
			{
				if (activityData is IDictionary<String, Object> expData)
				{
					if (expData.TryGetValue(name, out Object objVal))
						return (Func<JsValue, JsValue[], JsValue>)objVal;
				}
			}
			return null;
		}

		public void Execute(String refer, String name)
		{
			_deferredTarget.Refer = refer;
			//_tracker.Track(new ScriptTrackRecord(ScriptTrackAction.Execute, refer, name));
			var func = GetFunc(refer, name);
			if (func == null)
				throw new WorkflowException($"Script element {refer}.{name} not found");
			func(JsValue.Undefined, null);
		}

		public void ExecuteResult(String refer, String name, Object result)
		{
			_deferredTarget.Refer = refer;
			_tracker.Track(new ScriptTrackRecord(ScriptTrackAction.ExecuteResult, refer, name, result));
			var func = GetFunc(refer, name);
			if (func == null)
				throw new WorkflowException($"Script element {refer}.{name} not found");
			// result must be not null
			var arg = JsValue.FromObject(_engine, result ?? new ExpandoObject());
			func(JsValue.Undefined, new JsValue[] { arg });
		}
	}
}
