using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Activities
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class FlowActivity : FlowNode, IStorable
	{
		public IActivity Activity { get; set; }

		ExecutingAction _onComplete;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
		}
		#endregion

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			context.Schedule(Activity, OnChildComplete);
			return new ValueTask();
		}

		public override IEnumerable<IActivity> EnumChildren()
		{
			yield return Activity;
		}

		[StoreName("OnChildComplete")]
		ValueTask OnChildComplete(IExecutionContext context, IActivity activity)
		{
			if (Next != null)
				context.Schedule(Parent.FindNode(Next), _onComplete);
			else if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}
	}
}
