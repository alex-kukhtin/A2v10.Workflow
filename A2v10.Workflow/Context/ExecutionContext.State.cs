﻿
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public partial class ExecutionContext : IExecutionContext
	{
		ExpandoObject GetActivityStates()
		{
			var actState = new ExpandoObject();
			foreach (var (refer, activity) in _activities)
			{
				if (activity is IStorable storable)
				{
					ActivityStorage storage = new(StorageState.Storing, _activities);
					if (activity is ICanComplete canComplete && !canComplete.IsComplete)
						storable.Store(storage);
					if (storage.Value.IsNotEmpty())
						actState.Set(refer, storage.Value);
				}
			}
			if (actState.IsEmpty())
				return null;
			return actState;
		}

		void SetActivityStates(ExpandoObject state)
		{
			if (state == null)
				return;
			foreach (var refer in state.Keys())
			{
				if (_activities.TryGetValue(refer, out IActivity activity))
				{
					if (activity is IStorable storable)
					{
						var storage = new ActivityStorage(StorageState.Loading, _activities, state.Get<ExpandoObject>(refer));
						storable.Restore(storage);
					}
				}
			}
		}

		ExpandoObject GetScriptVariables()
		{
			var vars = new ExpandoObject();
			foreach (var (refer, activity) in _activities)
			{
				if (activity is IScoped)
					vars.SetNotNull(refer, _script.Evaluate<ExpandoObject>(refer, "Store"));
			}
			if (vars.IsEmpty())
				return null;
			return vars;
		}

		void SetScriptVariables(ExpandoObject vars)
		{
			if (vars == null)
				return;
			foreach (var refer in vars.Keys())
			{
				if (_activities.TryGetValue(refer, out IActivity activity))
					if (activity is IScoped)
						_script.Restore(refer, vars.Get<ExpandoObject>(refer));
			}
		}

		ExpandoObject GetEvents()
		{
			if (_events == null || _events.Count == 0)
				return null;
			var res = new ExpandoObject();
			foreach (var (k, v) in _events)
			{
				var eo = new ExpandoObject();
				eo.Set("Action", CallbackItem.CreateFrom(v.Action));
				eo.Set("Event", v.Event.ToExpando());
				res.Set(k, eo);
			}
			return res;
		}

		ExpandoObject GetBookmarks()
		{
			if (_bookmarks == null || _bookmarks.Count == 0)
				return null;
			var res = new ExpandoObject();
			foreach (var b in _bookmarks)
				res.Set(b.Key, CallbackItem.CreateFrom(b.Value));
			return res;
		}

		void SetBookmarks(ExpandoObject marks)
		{
			if (marks == null)
				return;
			foreach (var k in marks.Keys())
			{
				var ebm = marks.Get<ExpandoObject>(k);
				var cb = CallbackItem.FromExpando(ebm);
				if (_activities.TryGetValue(cb.Ref, out IActivity activity))
					_bookmarks.Add(k, cb.ToBookmark(activity));
				else
					throw new WorkflowException($"Activity {cb.Ref} for bookmark callback not found");
			}
		}

		void SetEvents(ExpandoObject events)
		{
			if (events == null)
				return;
			foreach (var k in events.Keys())
			{
				var ebm = events.Get<ExpandoObject>(k);
				var cb = CallbackItem.FromExpando(ebm.Get<ExpandoObject>("Action"));
				var wfe = WorkflowEventImpl.FromExpando(k, ebm.Get<ExpandoObject>("Event"));
				if (_activities.TryGetValue(cb.Ref, out IActivity activity))
					_events.Add(k, new EventItem(cb.ToEvent(activity), wfe));
				else
					throw new WorkflowException($"Activity {cb.Ref} for event callback not found");
			}
		}

		public ExpandoObject GetState()
		{
			var res = new ExpandoObject();
			res.SetNotNull("State", GetActivityStates());
			res.SetNotNull("Variables", GetScriptVariables());
			res.SetNotNull("Bookmarks", GetBookmarks());
			res.SetNotNull("Events", GetEvents());
			return res;
		}

		public ExpandoObject GetExternalVariables(ExpandoObject state)
		{
			List<IVariable> variables = null;
			if (_root is IExternalScoped extScoped)
				variables = extScoped.ExternalVariables();
			else if (_root is IScoped scoped)
				variables = scoped.Variables;
			if (variables == null || variables.Count == 0)
				return null;
			var result = new ExpandoObject();
			var values = state.Get<ExpandoObject>("Variables");
			var rootValues = values.Get<ExpandoObject>(_root.Id);

			void AddList(VariableType varType, String propName)
			{
				var list = new List<Object>();
				foreach (var v in variables.Where(v => v.External && v.Type == varType))
				{
					var ve = new ExpandoObject();
					ve.Set("Name", v.Name);
					if (v is IExternalVariable extVar)
					{
						var rv = values.Get<ExpandoObject>(extVar.ActivityId);
						ve.Set("Value", rv.Get<Object>(v.Name));
					}
					else
						ve.Set("Value", rootValues.Get<Object>(v.Name));
					list.Add(ve);
				}
				if (list.Count > 0)
					result.Set(propName, list);

			};

			AddList(VariableType.BigInt, "BigInt");
			AddList(VariableType.String, "String");
			AddList(VariableType.Guid, "Guid");
			if (result.IsEmpty())
				return null;
			return result;
		}

		public List<Object> GetExternalEvents()
		{
			if (_events == null || _events.Count == 0)
				return null;
			var list = new List<Object>();
			foreach (var (_, v) in _events)
				list.Add(v.Event.ToStore());
			return list;
		}

		public List<Object> GetExternalBookmarks()
		{
			if (_bookmarks == null || _bookmarks.Count == 0)
				return null;
			var list = new List<Object>();
			foreach (var b in _bookmarks)
				list.Add(
					new ExpandoObject() {
						{ "Bookmark", b.Key }
					}
				);
			return list;
		}

		public WorkflowExecutionStatus GetExecutionStatus()
		{
			WorkflowExecutionStatus status = WorkflowExecutionStatus.Complete;
			if (_bookmarks.Count > 0 || _events.Count > 0)
				status = WorkflowExecutionStatus.Idle;
			return status;
		}

		public void SetState(ExpandoObject state)
		{
			SetActivityStates(state.Get<ExpandoObject>("State"));
			SetScriptVariables(state.Get<ExpandoObject>("Variables"));
			SetBookmarks(state.Get<ExpandoObject>("Bookmarks"));
			SetEvents(state.Get<ExpandoObject>("Events"));
		}

		public List<Object> GetTrackRecords()
		{
			var records = _tracker.Records;
			if (records == null)
				return null;

			var lst = new List<Object>();
			for (var i=0; i<records.Count; i++)
				lst.Add(records[i].ToExpandoObject(i + 1 /*1-based*/));

			if (lst.Count == 0)
				return null;
			return lst;
		}
	}
}
