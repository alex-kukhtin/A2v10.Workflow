
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Workflow
{
	public static class ExpandoObjectExtensions
	{
		public static T Get<T>(this ExpandoObject expobj, String name)
		{
			var d = expobj as IDictionary<String, Object>;
			if (d.TryGetValue(name, out Object res))
				return (T)Convert.ChangeType(res, typeof(T));
			return default;
		}

		public static void Set<T>(this ExpandoObject expobj, String name, T value)
		{
			var d = expobj as IDictionary<String, Object>;
			d.Add(name, value);
		}

		public static void SetNotNull<T>(this ExpandoObject expobj, String name, T value) where T : class
		{
			if (value == null)
				return;
			var d = expobj as IDictionary<String, Object>;
			d.Add(name, value);
		}

		public static void SetOrReplace<T>(this ExpandoObject expobj, String name, T value)
		{
			var d = expobj as IDictionary<String, Object>;
			if (d.ContainsKey(name))
				d[name] = value;
			else
				d.Add(name, value);
		}

		public static IEnumerable<String> Keys(this ExpandoObject expobj)
		{
			return (expobj as IDictionary<String, Object>).Keys;
		}
	}
}
