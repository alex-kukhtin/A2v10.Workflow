using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Flowchart : Activity, IScoped
	{
		public List<FlowNode> Nodes { get; set; }
		public List<IVariable> Variables { get; set; }

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Nodes != null)
				foreach (var node in Nodes)
					yield return node;
		}

		public override void OnEndInit()
		{
			foreach (var node in Nodes)
				node.Parent = this;
		}

		public FlowNode FindNode(String refer)
		{
			return Nodes?.Find(node => node.Ref == refer);
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			if (Nodes == null)
			{
				if (onComplete != null)
					return onComplete(context, this);
				return new ValueTask();
			}
			var start = Nodes.Find(n => n.IsStart);
			if (start == null)
				throw new WorkflowExecption($"Flowchart (Ref={Ref}. Start node not found");
			context.Schedule(start, onComplete);
			return new ValueTask();
		}

		#region IScriptable
		public virtual void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
		}
		#endregion
	}
}
