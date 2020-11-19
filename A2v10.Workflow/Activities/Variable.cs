
using A2v10.Workflow.Interfaces;
using Newtonsoft.Json;
using System;

namespace A2v10.Workflow
{
	public class Variable : IVariable
	{
		public String Name { get; set; }
		public VariableDirection Dir { get; set; }
		public VariableType Type { get; set; }

		[JsonIgnore]
		public Boolean IsArgument => Dir == VariableDirection.In || Dir == VariableDirection.InOut;
		[JsonIgnore]
		public Boolean IsResult => Dir == VariableDirection.Out || Dir == VariableDirection.InOut;
	}
}
