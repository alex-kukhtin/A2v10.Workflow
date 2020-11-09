
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Parallel : Activity, IStorable, IHasContext, IScriptable
	{
		const String ON_COMPLETE = "OnComplete";

		public List<IVariable> Variables { get; set; }
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


		#region IScriptable
		public virtual void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
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

		public override void Traverse(TraverseArg traverse)
		{
			traverse.Start?.Invoke(this);
			if (Branches != null)
				foreach (var branch in Branches)
					branch.Traverse(traverse);
			traverse.End?.Invoke(this);
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
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
