using A2v10.Workflow.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class ScriptTask : BpmnTask, IScriptable
	{
		// bpmn:script
		public String Script => Children.OfType<A2v10.Workflow.Bpmn.Script>().FirstOrDefault()?.Text;

		public override ValueTask ExecuteBody(IExecutionContext context)
		{
			context.Execute(Id, nameof(Script));
			return CompleteBody(context);
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildExecute(nameof(Script), Script);
		}
		#endregion

	}
}
