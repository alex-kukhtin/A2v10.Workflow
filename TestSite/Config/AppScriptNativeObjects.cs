using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.SqlServer;

namespace TestSite.Config
{
	public class AppScriptNativeObjects : IScriptNativeObjectProvider
	{
		private readonly NativeType[] _nativeTypes = new NativeType[] {
			new NativeType() {Name = "Database", Type = typeof(ScriptNativeDatabase)}
		};

		public IEnumerable<NativeType> NativeTypes()
		{
			return _nativeTypes;
		}
	}
}
