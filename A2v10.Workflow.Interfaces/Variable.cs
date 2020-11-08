using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	public enum VariableDirection
	{
		Internal,
		Const,
		In, /*argument*/
		Out /*result*/
	}

	public enum VariableType
	{
		String,
		Number,
		Boolean,
		Object
	}

	public class Variable
	{
		public String Name { get; set; }
		public VariableDirection Dir { get; set; }
		public VariableType Type { get; set; }
	}
}
