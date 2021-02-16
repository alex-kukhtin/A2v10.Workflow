
using System;
using System.ComponentModel;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public record SpecialPropertyDescriptor(String Name);

	public record PropertyDescriptor
	{
		public PropertyInfo PropertyInfo { get; init; }
		public Type Type { get; init; }
		public Func<Object> Constructor { get; init; }
		public Action<Object, Object> AddMethod { get; init; }
		public Action<Object, String, Object> AddDictionaryMethod { get; init; }
		public TypeConverter TypeConverter { get; init; }

		public Boolean IsPlain => AddMethod == null && AddDictionaryMethod == null;
		public Boolean IsArray => AddMethod != null && Constructor != null;
		public Boolean IsDictionary => AddDictionaryMethod != null && Constructor != null;


		public Object BuildElement(NodeBuilder builder, XamlNode node)
		{
			if (IsPlain)
			{
				if (PropertyInfo.CanWrite)
					return XamlNode.GetNodeValue(builder, node);
				else
					throw new XamlReadException($"Property {PropertyInfo.PropertyType} is read only");
			}
			else if (IsDictionary && node.HasChildren)
			{
				if (!node.HasChildren)
					return null;
				var dict = Constructor();
				foreach (var nd in node.Children.Value)
				{
					var key = nd.Properties["Key"];
					if (key is SpecialPropertyDescriptor spec)
					{
						var dVal = builder.BuildNode(nd);
						AddDictionaryMethod(dict, spec.Name, dVal);
					}
				}
				return dict;
			}
			else if (IsArray)
			{
				if (!node.HasChildren)
					return null;
				var arr = Constructor();
				foreach (var nd in node.Children.Value)
					AddMethod(arr, builder.BuildNode(nd));
				return arr;
			}
			throw new NotImplementedException($"Property: {PropertyInfo.Name}");
		}
	}
}
