using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class Process : BpmnElement, IStorable
	{
		public Boolean IsExecutable { get; init; }
		public List<BpmnElement> Elements { get; init; }
		public List<IVariable> Variables { get; init; }

		private readonly List<IToken> _tokens = new List<IToken>();

		ExecutingAction _onComplete;
		IToken _token;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
		}
		#endregion

		#region IScriptable
		public void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
		}
		#endregion

		public override IEnumerable<IActivity> EnumChildren()
		{
			if (Elements != null)
				foreach (var elem in Elements)
					yield return elem;
		}

		public override ValueTask ExecuteAsync(IExecutionContext context, IToken token, ExecutingAction onComplete)
		{
			_onComplete = onComplete;
			_token = token;
			if (!IsExecutable || Elements == null)
				return new ValueTask();
			var start = Elems<Event>().FirstOrDefault(ev => ev.IsStart);
			if (start == null)
				throw new WorkflowExecption($"Process (Id={Id}). Start event not found");
			context.Schedule(start, OnElemComplete, token);
			return new ValueTask();
		}

		public IEnumerable<T> Elems<T>() where T : BpmnElement
			=> Elements.Where(e => e is T).Select(e => (T)e);

		[StoreName("OnElemComplete")]
		ValueTask OnElemComplete(IExecutionContext context, IActivity activity)
		{
			if (activity is EndEvent endEvent)
				return ProcessComplete(context, endEvent);
			return new ValueTask();
		}

		ValueTask ProcessComplete(IExecutionContext context, EndEvent endEvent)
		{
			if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}

		public override void OnEndInit()
		{
			if (Elements == null)
				return;
			foreach (var e in Elements)
				e.SetParent(this);
		}

		public T FindElement<T>(String id) where T : BpmnElement
		{
			var elem = Elements?.Find(e => e.Id == id);
			if (elem == null)
				throw new WorkflowExecption($"BPMN. Element (Id = {id}) not found");
			if (elem is T elemT)
				return elemT;
			throw new WorkflowExecption($"BPMN. Invalid type for element (Id = {id}). Expected: '{typeof(T).Name}', Actual: '{elem.GetType().Name}'");
		}

		public IToken NewToken()
		{
			var t = Token.Create();
			_tokens.Add(t);
			return t;
		}

		public void KillToken(IToken token)
		{
			_tokens.Remove(token);
		}
	}
}
