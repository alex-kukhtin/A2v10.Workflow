using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace A2v10.System.Xaml
{
	public static class PropertyConvertor
	{
		public static Object ConvertValue(Object value, Type type)
		{
			if (value == null)
				return null;
			if (value.GetType() == type)
				return value;
			if (value.GetType() == (Nullable.GetUnderlyingType(type) ?? type))
				return value;
			var conv = type.GetCustomAttribute<TypeConverterAttribute>();
			if (conv != null)
			{
				var convCtor = Type.GetType(conv.ConverterTypeName).GetConstructor(Array.Empty<Type>());
				var typeConverter = convCtor.Invoke(Array.Empty<Object>()) as TypeConverter;
				if (typeConverter.CanConvertFrom(value.GetType()))
					return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			return Convert.ChangeType(value, type);
		}
	}
}
