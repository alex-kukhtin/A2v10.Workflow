
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StateMachine : Activity, IStorable, IHasContext
	{
		public List<IVariable> Variables { get; set; }	
		public List<State> States { get; set; }

		String _currentState;
		ExecutingAction _onComplete;

		#region IStorable

		const String ON_COMPLETE = "OnComplete";
		const String CURRENT_STATE = "CurrentState";

		public void Store(IActivityStorage storage)
		{
			storage.Set(CURRENT_STATE, _currentState);
			storage.SetCallback(ON_COMPLETE, _onComplete);
		}
		public void Restore(IActivityStorage storage)
		{
			_currentState = storage.Get<String>(CURRENT_STATE);
			_onComplete = storage.GetCallback(ON_COMPLETE);
		}
		#endregion

		public override IEnumerable<IActivity> EnumChildren()
		{
			return Enumerable.Empty<IActivity>();
		}


		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			var state = States.Find(s => s.IsStart);
			if (state == null)
				throw new WorkflowExecption("Flowchart. Start node not found");
			_currentState = state.Ref;
			context.Schedule(state, OnNextState);
			return new ValueTask();
		}

		ValueTask OnNextState(IExecutionContext context, IActivity activity)
		{
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
