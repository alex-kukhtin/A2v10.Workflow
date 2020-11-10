
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Sequence : Activity, IStorable, IHasContext
	{
		public List<IActivity> Activities { get; set; }
		public List<IVariable> Variables { get; set; }

		ExecutingAction _onComplete;
		Int32 _next;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String NEXT = "Next";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.Set<Int32>(NEXT, _next);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_next = storage.Get<Int32>(NEXT);
		}
		#endregion

		#region IScriptable
		public virtual void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
		}
		#endregion


		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Activities != null)
			{
				foreach (var a in Activities)
					yield return a;
			}
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			if (Activities == null || Activities.Count == 0)
			{
				if (onComplete != null)
					return onComplete(context, this);
				return new ValueTask();
			}
			_next = 0;
			var first = Activities[_next++];
			context.Schedule(first, OnChildComplete);
			return new ValueTask();
		}

		[StoreName("OnChildComplete")]
		ValueTask OnChildComplete(IExecutionContext context, IActivity activity)
		{
			if (Activities == null || _next >= Activities.Count)
			{
				if (_onComplete != null)
					return _onComplete(context, this);
			}
			else
			{
				var next = Activities[_next++];
				context.Schedule(next, OnChildComplete);
			}
			return new ValueTask();
		}
	}
}
