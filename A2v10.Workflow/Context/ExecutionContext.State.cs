
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace A2v10.Workflow
{
	public partial class ExecutionContext : IExecutionContext
	{
		ExpandoObject GetActivityStates()
		{
			var actState = new ExpandoObject();
			var actStateD = actState as IDictionary<String, Object>;
			foreach (var (refer, activity) in _activities)
			{
				if (activity is IStorable storable)
				{
					ActivityStorage storage = new ActivityStorage(StorageState.Storing, _activities);
					storable.Store(storage);
					actStateD.Add(refer, storage.Value);
				}
			}
			if (actStateD.Count == 0)
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
					if (activity is IStorable storable)
					{
						var storage = new ActivityStorage(StorageState.Loading, _activities, state.Get<ExpandoObject>(refer));
						storable.Restore(storage);
					}
			}
		}

		ExpandoObject GetScriptVariables()
		{
			var vars = new ExpandoObject();
			var varsD = vars as IDictionary<String, Object>;
			foreach (var (refer, activity) in _activities)
			{
				if (activity is IScoped)
					varsD.Add(refer, _script.Evaluate<ExpandoObject>(refer, "Store"));
			}
			if (varsD.Count == 0)
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
					throw new WorkflowExecption($"Activity {cb.Ref} for bookmark callback not found");
			}

		}

		public ExpandoObject GetState()
		{
			var res = new ExpandoObject();
			res.SetNotNull("State", GetActivityStates());
			res.SetNotNull("Variables", GetScriptVariables());
			res.SetNotNull("Bookmarks", GetBookmarks());
			return res;
		}

		public ExpandoObject GetExternalVariables(ExpandoObject state)
		{
			if (_root is not IScoped scopedRoot)
				return null;
			var variables = scopedRoot.Variables;
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
					ve.Set("Value", rootValues.Get<Object>(v.Name));
					list.Add(ve);
				}
				if (list.Count > 0)
					result.Set(propName, list);

			};
			AddList(VariableType.BigInt, "BigInt");
			AddList(VariableType.String, "String");
			AddList(VariableType.Guid, "Guid");
			if (result.Any())
				return result;
			return null;
		}

		public ExpandoObject GetExternalBookmarks()
		{
			if (_bookmarks == null || _bookmarks.Count == 0)
				return null;
			var eo = new ExpandoObject();
			var list = new List<Object>();
			foreach (var b in _bookmarks)
			{
				var be = new ExpandoObject();
				be.Set("Bookmark", b.Key);
				list.Add(be);
			}
			eo.Set("Items", list);
			return eo;
		}

		public WorkflowExecutionStatus GetExecutionStatus()
		{
			WorkflowExecutionStatus status = WorkflowExecutionStatus.Complete;
			if (_bookmarks.Any())
				status = WorkflowExecutionStatus.Idle;
			return status;
		}

		public void SetState(ExpandoObject state)
		{
			SetActivityStates(state.Get<ExpandoObject>("State"));
			SetScriptVariables(state.Get<ExpandoObject>("Variables"));
			SetBookmarks(state.Get<ExpandoObject>("Bookmarks"));
		}
	}
}
