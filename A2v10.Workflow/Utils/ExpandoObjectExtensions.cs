﻿
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

		public static void Add(this ExpandoObject expobj, String name, Object value)
		{
			var d = expobj as IDictionary<String, Object>;
			d.Add(name, value);
		}

		public static ExpandoObject Clone(this ExpandoObject expobj)
		{
			var rv = new ExpandoObject();
			foreach (var (k, v) in expobj)
			{
				if (v is ExpandoObject vexp)
					rv.Add(k, vexp.Clone());
				else
					rv.Add(k, v);
			}
			return rv;
		}

		public static void Set<T>(this ExpandoObject expobj, String name, T value)
		{
			var d = expobj as IDictionary<String, Object>;
			d.Add(name, value);
		}

		public static Boolean IsEmpty(this ExpandoObject expobj)
		{
			return expobj == null || (expobj as IDictionary<String, Object>).Count == 0;
		}

		public static Boolean IsNotEmpty(this ExpandoObject expobj)
		{
			return expobj != null && (expobj as IDictionary<String, Object>).Count > 0;
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

		public static String SerializeSimple(this ExpandoObject expobj)
		{
			if (expobj == null)
				return null;

			static String ToString(Object val)
			{
				if (val == null)
					return null;
				if (val is String)
					return $"'{val.ToString().Replace("'", "\\'")}'";
				return val.ToString();
			}

			List<String> arr = new List<String>();
			foreach (var (k, v) in expobj)
			{
				arr.Add($"{k}:{ToString(v)}");
			}
			return $"{{{String.Join(", ", arr)}}}";
		}

	}
}
