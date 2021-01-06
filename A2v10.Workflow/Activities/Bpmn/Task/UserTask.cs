
using System;
using System.Linq;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class UserTask : BpmnTask, IScriptable
	{
		public String Script => Children.OfType<Script>().FirstOrDefault()?.Text;

		public override ValueTask ExecuteBody(IExecutionContext context)
		{
			context.SetBookmark(Id, OnUserTaskComplete);
			return new ValueTask();
		}

		public override void Store(IActivityStorage storage)
		{
			base.Store(storage);
		}

		public override void Restore(IActivityStorage storage)
		{
			base.Restore(storage);
		}

		[StoreName("OnUserTaskComplete")]
		ValueTask OnUserTaskComplete(IExecutionContext context, String bookmark, Object result)
		{
			context.RemoveBookmark(bookmark);
			context.ExecuteResult(Id, nameof(Script), result);
			CompleteBody(context);
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
