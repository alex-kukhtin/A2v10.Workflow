
using System;
using System.Collections.Generic;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public class TypeDescriptor
	{
		public String TypeName {get; init;}
		public Func<Object> Constructor { get; init; }
		public Func<String, Object> ConstructorString { get; init; }
		public Dictionary<String, PropertyInfo> Properties { get; init; }

		public String ContentProperty { get; init; }
		public String DefaultProperty { get; init; }

		public MethodInfo AddCollection1 { get; init; }
		public MethodInfo AddCollection2 { get; init; }

		public void SetPropertyValue(Object instance, String name, Object value)
		{
			if (!Properties.TryGetValue(name, out PropertyInfo propInfo))
				throw new XamlException($"Property {name} not found in type {TypeName}");
			var val = PropertyConvertor.ConvertValue(value, propInfo.PropertyType);
			propInfo.SetValue(instance, val);
		}

		public void SetTextContent(Object instance, String content)
		{
			if (ContentProperty == null)
				throw new XamlException($"ContentProperty not found in type {TypeName}");
			if (String.IsNullOrEmpty(content))
				return;
			if (!Properties.TryGetValue(ContentProperty, out PropertyInfo contProp))
				throw new XamlException($"Property {ContentProperty} not found in type {TypeName}");
			var val = PropertyConvertor.ConvertValue(content, contProp.PropertyType);
			contProp.SetValue(instance, val);
		}

		public void AddChildren(Object instance, Object elem)
		{
			if (!Properties.TryGetValue(ContentProperty, out PropertyInfo contProp))
				return;
			var contObj = contProp.GetValue(instance);
			if (contObj == null)
			{
				var ctor = contProp.PropertyType.GetConstructor(Array.Empty<Type>());
				contObj = ctor.Invoke(Array.Empty<Object>());
				contProp.SetValue(instance, contObj);
			}
			if (AddCollection1 != null)
				AddCollection1.Invoke(contObj, new Object[] { elem });
		}

		public void AddExtension(Object instance, XamlExtensionElem ext)
		{
			//IProvideValueTarget
			//int z = 55;
		}
	}
}
