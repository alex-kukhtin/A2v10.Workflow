
using System;
using System.Collections.Generic;

namespace A2v10.Workflow.Interfaces
{
	public interface IScoped : IScriptable
	{
		List<IVariable> Variables { get; }
	}
	
	public interface IExternalScoped
	{
		List<IVariable> ExternalVariables();
	}
}
