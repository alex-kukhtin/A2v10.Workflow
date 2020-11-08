
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Parallel : Activity, IStorable
	{
		const String ON_COMPLETE = "OnComplete";

		public List<IActivity> Branches { get; set; }

		#region storable
		ExecutingAction _onComplete;
		#endregion

		#region IStorable
		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
		}
		#endregion

		public async override ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			await base.TraverseAsync(onAction);
			if (Branches == null)
				return;
			foreach (var branch in Branches)
				await onAction(branch);
		}

		public override void Traverse(Action<IActivity> onAction)
		{
			base.Traverse(onAction);
			if (Branches == null)
				return;
			foreach (var branch in Branches)
				onAction(branch);
		}

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			if (Branches == null || Branches.Count == 0)
			{
				if (onComplete != null)
					return onComplete(context, this);
				return new ValueTask();
			}
			foreach (var br in Branches)
				context.Schedule(br, OnBranchComplete);
			return new ValueTask();
		}

		[StoreName("OnBranchComplete")]
		ValueTask OnBranchComplete(IExecutionContext context, IActivity activity)
		{
			// TODO: cancel
			if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}
	}
}
