using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Transition : ActivityWithComplete, IScriptable
	{
		public String Condition { get; set; }

		public IActivity Trigger { get; set; }
		public IActivity Action { get; set; }

		public String Destination { get; set; }

		internal String NextState { get; set; }

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			NextState = null;
			if (Trigger != null)
				context.Schedule(Trigger, OnTriggerComplete);
			else
				return ContinueExecute(context);
			return new ValueTask();
		}

		[StoreName("OnTriggerComplete")]
		ValueTask OnTriggerComplete(IExecutionContext context, IActivity activity)
		{
			return ContinueExecute(context);
		}

		ValueTask ContinueExecute(IExecutionContext context)
		{
			var cond = context.Evaluate<Boolean>(Ref, nameof(Condition));
			if (cond)
			{
				NextState = Destination;
				if (Action != null)
				{
					context.Schedule(Action, _onComplete);
					return new ValueTask();
				}
			} 
			if (_onComplete != null)
			{
				return _onComplete(context, this);
			}
			return new ValueTask();
		}

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Trigger != null)
				yield return Trigger;
			if (Action != null)
				yield return Action;
		}

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.BuildEvaluate(nameof(Condition), Condition);
		}
		#endregion
	}
}
