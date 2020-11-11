
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class State : StateBase, IStorable
	{
		public IActivity Entry { get; set; }
		public IActivity Exit { get; set; }

		public List<Transition> Transitions { get; set; }

		ExecutingAction _onComplete;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
		}
		#endregion

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Entry != null)
				yield return Entry;
			if (Transitions != null)
				foreach (var tr in Transitions)
					yield return tr;
			if (Exit != null)
				yield return Exit;
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			NextState = null;
			if (Entry != null)
				context.Schedule(Entry, OnEntryComplete);
			else if (ScheduleTransitions(context))
				return new ValueTask();
			else
				return ScheduleExit(context, Next);
			return new ValueTask();
		}

		// Schedule all transitions.
		Boolean ScheduleTransitions(IExecutionContext context)
		{
			if (Transitions == null || Transitions.Count == 0)
				return false;
			foreach (var tr in Transitions)
				context.Schedule(tr, OnTransitionComplete);
			return true;
		}

		ValueTask ScheduleExit(IExecutionContext context, String nextState)
		{
			NextState = nextState;
			if (Exit != null)
				context.Schedule(Exit, _onComplete);
			else if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}

		[StoreName("OnEntryComplete")]
		ValueTask OnEntryComplete(IExecutionContext context, IActivity activity)
		{
			if (ScheduleTransitions(context))
				return new ValueTask();
			else
				return ScheduleExit(context, Next);
		}

		[StoreName("OnTransitionComplete")]
		ValueTask OnTransitionComplete(IExecutionContext context, IActivity activity)
		{
			if (activity is not Transition tr)
				throw new InvalidProgramException("Invalid cast 'Transition'");
			if (tr.NextState != null)
			{
				// Transition completed. Returning with new state.
				NextState = tr.NextState;
				return ScheduleExit(context, tr.NextState);
			}
			return new ValueTask();
		}
	}
}