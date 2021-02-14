
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
			
			var valueType = value.GetType();

			type = Nullable.GetUnderlyingType(type) ?? type;

			if (valueType == type || type == typeof(Object))
				return value;

			if (valueType.IsAssignableTo(type))
				return value;
			
			if (type.IsEnum)
				return Enum.Parse(type, value.ToString());
			var conv = type.GetCustomAttribute<TypeConverterAttribute>();
			if (conv != null)
			{
				var convCtor = Type.GetType(conv.ConverterTypeName).GetConstructor(Array.Empty<Type>());
				var typeConverter = convCtor.Invoke(Array.Empty<Object>()) as TypeConverter;
				if (typeConverter.CanConvertFrom(valueType))
					return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}
	}
}
