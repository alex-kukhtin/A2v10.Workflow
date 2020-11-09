using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace A2v10.Workflow.Tests
{
	public static class ExpandoObjectExtensions
	{
		public static T Get<T>(this ExpandoObject expobj, String name)
		{
			var d = expobj as IDictionary<String, Object>;
			if (d.TryGetValue(name, out Object res))
				return (T) Convert.ChangeType(res, typeof(T));
			return default;
		}
	}
}
