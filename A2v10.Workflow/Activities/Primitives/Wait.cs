
using A2v10.Workflow.Interfaces;
using System;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Wait : ActivityWithComplete
	{
		public String Bookmark { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			context.SetBookmark(Bookmark, OnBookmarkComplete);
			return ValueTask.CompletedTask;
		}

		[StoreName("OnBookmarkComplete")]
		ValueTask OnBookmarkComplete(IExecutionContext context, String bookmark, Object result)
		{
			context.RemoveBookmark(bookmark);
			if (_onComplete != null)
				return _onComplete(context, this);
			return ValueTask.CompletedTask;
		}

	}
}
