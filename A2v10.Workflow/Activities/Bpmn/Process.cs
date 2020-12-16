using System;
using System.Linq;

using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.System.Xaml;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	[ContentProperty("Elements")]
	public class Process : BpmnActivity, IStorable, IScoped, IScriptable
	{
		public Boolean IsExecutable { get; init; }
		public List<BpmnItem> Elements { get; init; }

		public List<IVariable> Variables => Elem<ExtensionElements>()?.GetVariables();

		private readonly List<IToken> _tokens = new List<IToken>();

		public IEnumerable<BpmnActivity> Activities => Elems<BpmnActivity>().ToList();

		ExecutingAction _onComplete;
		IToken _token;

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";
		const String TOKENS = "Tokens";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
			//TODO:storage.Set(TOKENS, _tokens);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
			//TODO:_tokens = storage.Get<List<IToken>>(TOKENS);
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
				foreach (var elem in Activities)
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

		public IEnumerable<T> Elems<T>() where T : BpmnItem => Elements.OfType<T>();

		public T Elem<T>() where T : BpmnItem => Elements.OfType<T>().FirstOrDefault();

		[StoreName("OnElemComplete")]
		ValueTask OnElemComplete(IExecutionContext context, IActivity activity)
		{
			if (activity is EndEvent)
				return ProcessComplete(context);
			return new ValueTask();
		}

		ValueTask ProcessComplete(IExecutionContext context)
		{
			if (_onComplete != null)
				return _onComplete(context, this);
			return new ValueTask();
		}

		public override void OnEndInit()
		{
			if (Elements == null)
				return;
			foreach (var e in Activities)
				e.SetParent(this);
		}

		public T FindElement<T>(String id) where T : BpmnActivity
		{
			var elem = Activities.FirstOrDefault(e => e.Id == id);
			if (elem == null)
				throw new WorkflowExecption($"BPMN. Element (Id = {id}) not found");
			if (elem is T elemT)
				return elemT;
			throw new WorkflowExecption($"BPMN. Invalid type for element (Id = {id}). Expected: '{typeof(T).Name}', Actual: '{elem.GetType().Name}'");
		}

		public IEnumerable<T> FindAll<T>(Predicate<T> predicate) where T : BpmnActivity
		{
			var list = Activities.Where(elem => elem is T t && predicate(t));
			foreach (var el in list)
				yield return el as T;
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
