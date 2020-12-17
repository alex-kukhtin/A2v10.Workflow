using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.System.Xaml;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class UserTask : BpmnTask, IScriptable
	{
		public String Script => Children.OfType<Script>().FirstOrDefault()?.Text;

		const String ON_USERTASK_COMPLETE = "OnUserTaskComplete";
		ExecutingAction _onUserTaskComplete;

		public override ValueTask ExecuteBody(IExecutionContext context, ExecutingAction onComplete)
		{
			_onUserTaskComplete = onComplete;
			context.SetBookmark(Id, OnUserTaskComplete);
			return new ValueTask();
		}

		public override void Store(IActivityStorage storage)
		{
			base.Store(storage);
			storage.SetCallback(ON_USERTASK_COMPLETE, _onUserTaskComplete);
		}

		public override void Restore(IActivityStorage storage)
		{
			base.Restore(storage);
			_onUserTaskComplete = storage.GetCallback(ON_USERTASK_COMPLETE);
		}

		[StoreName("OnUserTaskComplete")]
		ValueTask OnUserTaskComplete(IExecutionContext context, String bookmark, Object result)
		{
			context.RemoveBookmark(bookmark);
			context.ExecuteResult(Id, nameof(Script), result);
			if (_onUserTaskComplete != null)
				return _onUserTaskComplete(context, this);
			return new ValueTask();
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecuteResult(nameof(Script), Script);
		}
		#endregion
	}
}
