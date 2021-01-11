using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public record NativeType
	{
		public Type Type { get; init; }
		public String Name { get; init; }
	}

	public interface IScriptNativeObjectProvider
	{
		IEnumerable<NativeType> NativeTypes();
	}
}
