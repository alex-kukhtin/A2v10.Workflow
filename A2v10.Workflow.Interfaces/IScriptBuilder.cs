using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	public interface IScriptBuilder
	{
		void AddVariables(IEnumerable<IVariable> variables);
		void BuildExecute(String name, String expression);
	}

	public interface IScriptable
	{
		void BuildScript(IScriptBuilder builder);
	}
}
