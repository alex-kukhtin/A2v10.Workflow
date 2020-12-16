using A2v10.Workflow.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class ScriptTask : BpmnTask, IScriptable
	{
		public String Script => Children.OfType<Script>().FirstOrDefault()?.Text;

		public override ValueTask ExecuteBody(IExecutionContext context, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			context.Execute(Id, nameof(Script));
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecute(nameof(Script), Script);
		}
		#endregion

	}
}
