using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BpmnTask : FlowElement, IStorable
	{

		protected ExecutingAction _onComplete;
		protected IToken _token;

		#region IStorable 
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";

		public virtual void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
		}

		public virtual void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
		}
		#endregion



		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			_token = token;
			// boundary events
			foreach (var ev in Parent.FindAll<BoundaryEvent>(ev => ev.AttachedToRef == Id))
			{
			}
			// loop
			return  ExecuteBody(context);
		}


		protected virtual ValueTask CompleteBody(IExecutionContext context)
		{
			if (Outgoing == null)
			{
				if (_onComplete != null)
					return _onComplete(context, this);
				return new ValueTask();
			}

			if (Outgoing.Count() == 1)
			{
				// simple outgouning - same token
				var targetFlow = Parent.FindElement<SequenceFlow>(Outgoing.First().Text);
				context.Schedule(targetFlow, null, _token);
			}
			else
			{
				// same as task + parallelGateway
				Parent.KillToken(_token);
				foreach (var flowId in Outgoing)
				{
					var targetFlow = Parent.FindElement<SequenceFlow>(flowId.Text);
					context.Schedule(targetFlow, null, Parent.NewToken());
				}
			}
			if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}

		public virtual ValueTask ExecuteBody(IExecutionContext context)
		{
			return CompleteBody(context);
		}
	}
}
