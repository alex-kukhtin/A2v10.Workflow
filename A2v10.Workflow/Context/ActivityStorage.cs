
using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public enum StorageState
	{
		Loading,
		Storing
	}

	public class ActivityStorage : IActivityStorage
	{
		private readonly ExpandoObject _expando;
		private readonly IDictionary<String, IActivity> _activities;

		public Boolean IsLoading { get; set; }
		public Boolean IsStoring => !IsLoading;

		public ExpandoObject Value => _expando;

		public ActivityStorage(StorageState state, IDictionary<String, IActivity> activities, ExpandoObject obj = null)
		{
			IsLoading = state == StorageState.Loading;
			_activities = activities;
			_expando = obj ?? new ExpandoObject();
		}

		public T Get<T>(String name)
		{
			return _expando.Get<T>(name);
		}

		public ExecutingAction GetCallback(String name)
		{
			var itm = _expando.Get<ExpandoObject>(name);
			if (itm == null)
				return null;
			var cb = CallbackItem.FromExpando(itm);
			if (!_activities.TryGetValue(cb.Ref, out IActivity activity))
				throw new WorkflowExecption($"Activity {cb.Ref} not found");
			return cb.ToCallback(activity);
		}

		public void Set<T>(String name, T value)
		{
			_expando.Set(name, value);
		}

		public void SetCallback(String name, ExecutingAction callback)
		{
			if (callback == null)
				return;
			_expando.Set(name, CallbackItem.CreateFrom(callback));
		}

		public void SetToken(String name, IToken value)
		{
			// TODO:
		}

		public IToken GetToken(String name)
		{
			// TODO:
			return null;
		}

	}
}
