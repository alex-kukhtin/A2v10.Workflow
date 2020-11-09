using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	public enum VariableType
	{
		String,
		Number,
		Boolean,
		Object
	}

	public enum VariableDirection
	{
		Local,
		Const,
		In, /* argument */
		Out, /* result */
		InOut /* bidirectional */
	}

	public interface IVariable
	{
		VariableType Type { get; }
		public VariableDirection Dir { get; set; }
		String Name { get; }

		public Boolean IsArgument { get; }
		public Boolean IsResult { get; }
	}
}
