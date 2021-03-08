using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public abstract class ProcessBase : BpmnActivity, IStorable, IScoped, IScriptable
	{

		protected ExecutingAction _onComplete;
		protected IToken _token;
		private readonly List<IToken> _tokens = new();

		#region IScoped
		public List<IVariable> Variables => Elem<ExtensionElements>()?.GetVariables();
		public String GlobalScript => Elem<ExtensionElements>()?.GetGlobalScript();

		public void BuildScript(IScriptBuilder builder)
		{
			builder.AddVariables(Variables);
		}
		#endregion

		#region IStorable
		const String ON_COMPLETE = "OnComplete";
		const String TOKEN = "Token";
		const String TOKENS = "Tokens";

		public void Store(IActivityStorage storage)
		{
			storage.SetCallback(ON_COMPLETE, _onComplete);
			storage.SetToken(TOKEN, _token);
			storage.SetTokenList(TOKENS, _tokens);
		}

		public void Restore(IActivityStorage storage)
		{
			_onComplete = storage.GetCallback(ON_COMPLETE);
			_token = storage.GetToken(TOKEN);
			storage.GetTokenList(TOKENS, _tokens);
		}
		#endregion

		public IToken NewToken()
		{
			var t = Token.Create();
			_tokens.Add(t);
			return t;
		}

		public void KillToken(IToken token)
		{
			if (token != null)
				_tokens.Remove(token);
		}
	}
}
