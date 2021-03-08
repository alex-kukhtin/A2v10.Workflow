
using System;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BoundaryEvent : Event, IStorable, ICanComplete
	{
		public String AttachedToRef { get; init; }
		public Boolean? CancelActivity { get; init; } // default is true

		public Boolean IsComplete { get; private set; }

		private ExecutingAction _onComplete;
		private IToken _token;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";

		public void Store(IActivityStorage storage)
		{
			if (IsComplete)
				return;
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
		}
		#endregion

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			_token = token;
			var eventDef = EventDefinition;
			if (eventDef != null)
				context.AddEvent(eventDef.CreateEvent(Id), this, OnTrigger);
			else
				SetComplete(context);
			return ValueTask.CompletedTask;
		}

		public override void Cancel(IExecutionContext context)
		{
			SetComplete(context);
		}

		[StoreName("OnTrigger")]
		public ValueTask OnTrigger(IExecutionContext context, IWorkflowEvent wfEvent, Object result)
		{
			foreach (var o in Outgoing)
			{
				var targetFlow = Parent.FindElement<SequenceFlow>(o.Text);
				context.Schedule(targetFlow, _onComplete, _token);
			}
			if (CancelActivity == null || CancelActivity.Value)
			{
				var task = Parent.FindElement<BpmnTask>(AttachedToRef);
				SetComplete(context);
				task.Cancel(context);
			} 
			else
			{
				var eventDef = EventDefinition;
				if (eventDef != null && eventDef.CanRepeat)
					context.AddEvent(eventDef.CreateEvent(Id), this, OnTrigger);
				else
					SetComplete(context);
			}
			return ValueTask.CompletedTask;
		}

		void SetComplete(IExecutionContext context)
		{
			IsComplete = true;
			context.RemoveEvent(Id);
			Parent.KillToken(_token);
		}
	}

}
