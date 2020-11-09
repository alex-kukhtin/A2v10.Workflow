
using System;
using System.Collections.Generic;
using System.Dynamic;

using A2v10.Workflow.Interfaces;

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
					ActivityStorage storage = new ActivityStorage(_activities);
					storable.Store(storage);
					actStateD.Add(refer, storage.Value);
				}
			}
			return actState;
		}

		void SetActivityStates(ExpandoObject state)
		{
			foreach (var refer in state.Keys())
			{
				if (_activities.TryGetValue(refer, out IActivity activity))
					if (activity is IStorable storable) {
						var storage = new ActivityStorage(_activities, state.Get<ExpandoObject>(refer));
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
				if (activity is IHasContext)
					varsD.Add(refer, _script.Evaluate<ExpandoObject>(refer, "Store"));
			}
			return vars;
		}

		void SetScriptVariables(ExpandoObject vars)
		{
			foreach (var refer in vars.Keys())
			{
				if (_activities.TryGetValue(refer, out IActivity activity))
					if (activity is IHasContext contextActivity)
						_script.Restore(refer, vars.Get<ExpandoObject>(refer));
			}
		}

		ExpandoObject GetBookmarks()
		{
			var res = new ExpandoObject();
			foreach (var b in _bookmarks)
				res.Set(b.Key, CallbackItem.CreateFrom(b.Value));
			return res;
		}

		void SetBookmarks(ExpandoObject marks)
		{
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
			res.Set("State", GetActivityStates());
			res.Set("Variables", GetScriptVariables());
			res.Set("Bookmarks", GetBookmarks());
			return res;
		}

		public void SetState(ExpandoObject state)
		{
			SetActivityStates(state.Get<ExpandoObject>("State"));
			SetScriptVariables(state.Get<ExpandoObject>("Variables"));
			SetBookmarks(state.Get<ExpandoObject>("Bookmarks"));
		}
	}
}
