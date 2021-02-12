
using System.Collections.Generic;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow;
using A2v10.Workflow.SqlServer;

namespace TestSite.Config
{
	public class AppScriptNativeObjects : IScriptNativeObjectProvider
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
