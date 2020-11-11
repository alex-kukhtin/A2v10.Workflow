
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

		public List<StateBase> States { get; set; }

		String _currentState; // for debug only
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
			if (States != null)
				foreach (var state in States)
					yield return state;
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			var startNode = States.Find(s => s.IsStart);
			if (startNode == null)
				throw new WorkflowExecption("Flowchart. Start node not found");
			_currentState = startNode.Ref;
			context.Schedule(startNode, OnNextState);
			return new ValueTask();
		}

		[StoreName("OnNextState")]
		ValueTask OnNextState(IExecutionContext context, IActivity activity)
		{
			if (!(activity is StateBase stateBase))
				throw new InvalidProgramException("Invalid cast 'StateBase'");
			var nextState = States.Find(st => st.Ref == stateBase.NextState);
			if (nextState != null)
			{
				_currentState = nextState.NextState;
				context.Schedule(nextState, OnNextState);
			}
			else if (_onComplete != null)
				return _onComplete(context, this);
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
