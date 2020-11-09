using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Wait : Activity, IStorable
	{
		public String Bookmark { get; set; }

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
			context.SetBookmark(Bookmark, OnBookmarkComplete);
			return new ValueTask();
		}


		[StoreName("OnBookmarkComplete")]
		ValueTask OnBookmarkComplete(IExecutionContext context, String Bookmark, Object result)
		{
			if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}

	}
}
