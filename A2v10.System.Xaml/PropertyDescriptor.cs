
using System;
using System.ComponentModel;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public record PropertyDescriptor
	{
		public PropertyInfo PropertyInfo { get; init; }
		public Type Type { get; init; }
		public Func<Object> Constructor { get; init; }
		public Action<Object, Object> AddMethod { get; init; }
		public Action<Object, String, Object> AddDictionaryMethod { get; init; }
		public Func<TypeConverter> TypeConverter { get; init; }
	}
}
