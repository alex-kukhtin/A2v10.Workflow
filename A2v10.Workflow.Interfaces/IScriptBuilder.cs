﻿using System;
using System.Collections.Generic;

namespace A2v10.Workflow.Interfaces
{
	public interface IScriptBuilder
	{
		void AddVariables(IEnumerable<IVariable> variables);
		void BuildExecute(String name, String expression);
		void BuildEvaluate(String name, String expression);
		void BuildExecuteResult(String name, String expression);
	}

	public interface IScriptable
	{
		void BuildScript(IScriptBuilder builder);
	}
}
