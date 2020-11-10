
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	public interface IHasContext : IScriptable
	{
		List<IVariable> Variables { get;}
	}
}
