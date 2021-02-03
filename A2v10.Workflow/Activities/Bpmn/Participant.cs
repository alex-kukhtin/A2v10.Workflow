using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Participant : BpmnActivity, IScoped
	{
		public String ProcessRef { get; init; }

		#region IScoped
		public List<IVariable> Variables => Elem<ExtensionElements>()?.GetVariables();

		public void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
		}
		#endregion

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Children != null)
				foreach (var elem in Children.OfType<Process>())
					yield return elem;
		}

		internal void EnsureChildren()
		{
			if (Children == null)
				Children = new List<BaseElement>();
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			var process = Children.OfType<Process>().FirstOrDefault(itm => itm.Id == ProcessRef);
			if (process == null)
				throw new WorkflowExecption($"Process '{ProcessRef}' not found");
			return process.ExecuteAsync(context, token, onComplete);
		}
	}
}
