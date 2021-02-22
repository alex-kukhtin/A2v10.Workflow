using System;
using System.Collections.Generic;

namespace A2v10.System.Xaml
{

	public class XamlAttachedPropertyManager : IAttachedPropertyManager
	{

		public record PropertyDef(String Name, Object Value);

		private readonly Dictionary<PropertyDef, Object> _map = new Dictionary<PropertyDef, Object>();

		public void SetProperty(String propName, Object obj, Object value)
		{
			_map.Add(new PropertyDef(propName, obj), value);
		}

		public T GetProperty<T>(String propName, Object obj)
		{
			if (_map.TryGetValue(new PropertyDef(propName, obj), out Object value))
				return (T) PropertyConvertor.ConvertValue(value, typeof(T), null);
			return default;
		}
	}
}
