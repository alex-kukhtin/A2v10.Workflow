
using System;
using System.ComponentModel;
using System.Reflection;
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
			
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum)
				return Enum.Parse(type, value.ToString());
			var conv = type.GetCustomAttribute<TypeConverterAttribute>();
			if (conv != null)
			{
				var convCtor = Type.GetType(conv.ConverterTypeName).GetConstructor(Array.Empty<Type>());
				var typeConverter = convCtor.Invoke(Array.Empty<Object>()) as TypeConverter;
				if (typeConverter.CanConvertFrom(value.GetType()))
					return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}
	}
}
