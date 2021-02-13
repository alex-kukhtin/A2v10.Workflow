using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Markup;

namespace A2v10.System.Xaml
{
	public record NodeDefinition
	{
		public String ClassName { get; init; }
		public Func<XamlNode, NodeDefinition, Object> Lambda { get; init; }
		public Dictionary<String, PropDefinition> Properties { get; init; }
		public String ContentProperty { get; init; }
		public String DefaultProperty { get; init; }
		public Func<XamlNode, Object> BuildNode { get; init; }
		public Type NodeType { get; init; }
		public Boolean IsCamelCase { get; init; }

		public Object BuildProperty(String name, Object value)
		{
			name = MakeName(name);
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found in type {ClassName}");
			if (value == null)
				return null;
			if (value.GetType() == propDef.Type)
				return value;
			if (propDef.EnumConvert != null)
				return propDef.EnumConvert(value.ToString());
			if (propDef.ScalarConvert != null)
				return propDef.ScalarConvert(value.ToString());
			throw new NotImplementedException($"Property {name} not implemented");
		}

		public Object BuildPropertyNode(NodeBuilder builder, String name, XamlNode node)
		{
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found");
			if (node == null)
				return null;
			if (propDef.Constructor != null)
			{
				var obj = propDef.Constructor();
				if (propDef.AddMethod != null)
				{
					if (node.HasChildren)
						foreach (var nd in node.Children.Value)
							propDef.AddMethod(obj, builder.BuildNode(nd));
				}
				return obj;
			}
			else if (propDef.EnumConvert != null)
				return propDef.EnumConvert(node.TextContent);
			else if (propDef.ScalarConvert != null)
			{
				var nval = GetNodeValue(builder, node);
				if (nval.GetType() == propDef.Type)
					return nval;
				return propDef.ScalarConvert(nval?.ToString());
			}
			else
				return GetNodeValue(builder, node);
		}

		static Object GetNodeValue(NodeBuilder builder, XamlNode node)
		{
			if (!node.HasChildren)
				return node.TextContent;
			if (node.Name.Contains('.'))
			{
				var ch = node.Children.Value[0];
				return builder.BuildNode(ch);
			}
			return node.TextContent;
		}

		public String MakeName(String name)
		{
			if (IsCamelCase)
			{
				// source: camelCase
				// code: PascalCase
				return name.ToPascalCase();
			}
			return name;
		}

		public PropertyInfo GetPropertyInfo(String name)
		{
			if (Properties.TryGetValue(name, out PropDefinition propDef))
				return propDef.PropertyInfo;
			throw new XamlReadException($"Property {name} not found");
		}
	}
}
