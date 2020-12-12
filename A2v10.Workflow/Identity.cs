using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public record Identity : IIdentity
	{
		public String Id { get; init; }
		public Int32 Version { get; init; }
	}
}
