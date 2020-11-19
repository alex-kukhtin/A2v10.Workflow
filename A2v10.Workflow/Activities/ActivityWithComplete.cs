﻿using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class ActivityWithComplete : Activity, IStorable
	{
		protected ExecutingAction _onComplete;
		protected IToken _token;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
			OnStore(storage);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
			OnRestore(storage);
		}
		#endregion

		public virtual void OnStore(IActivityStorage storage)
		{
		}

		public virtual void OnRestore(IActivityStorage storage)
		{
		}
	}
}
