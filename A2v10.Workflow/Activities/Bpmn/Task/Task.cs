using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class BpmnTask : FlowElement
	{

		ExecutingAction _onComplete;
		IToken _token;

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			_token = token;
			// boundary events
			foreach (var ev in Parent.FindAll<BoundaryEvent>(ev => ev.AttachedToRef == Id))
			{

			}
			return  ExecuteBody(context, OnBodyComplete);
		}


		ValueTask OnBodyComplete(IExecutionContext context, IActivity activity)
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

		public virtual ValueTask ExecuteBody(IExecutionContext context, ExecutingAction onComplete)
		{
			if (onComplete != null)
				return onComplete(context, this);
			return new ValueTask();
		}
	}
}
