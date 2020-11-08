
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class StateMachine : Activity, IStorable, IHasVariables
	{
		public List<Variable> Variables { get; set; }
	
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

		public override ValueTask Execute(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			var state = States.Find(s => s.IsStart);
			_currentState = state.Ref;
			context.Schedule(state, OnNextState);
			return new ValueTask();
		}

		ValueTask OnNextState(IExecutionContext context, IActivity activity)
		{
			return new ValueTask();
		}

		#region Traverse
		public override void Traverse(Action<IActivity> onAction)
		{
			base.Traverse(onAction);
		}

		public override ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			return base.TraverseAsync(onAction);
		}
		#endregion
	}
}
