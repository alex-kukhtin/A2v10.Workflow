
using System.Collections.Generic;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.SqlServer;

namespace A2v10.Workflow.WebHost
{
	public class ScriptNativeObjects : IScriptNativeObjectProvider
	{
		private readonly NativeType[] _nativeTypes = new NativeType[] {
			new NativeType() {Name = "Database", Type = typeof(ScriptNativeDatabase)},
			new NativeType() {Name = "Deferred", Type = typeof(ScriptNativeDeferred)}
		};

		public IEnumerable<NativeType> NativeTypes()
		{
			return _nativeTypes;
		}
	}
}
