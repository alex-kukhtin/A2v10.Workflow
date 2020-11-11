using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class ActivityWithComplete : Activity, IStorable
	{
		protected ExecutingAction _onComplete;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			OnStore(storage);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
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
