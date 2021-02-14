using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public record NodeDefinition
	{
		public String ClassName { get; init; }
		public Dictionary<String, PropDefinition> Properties { get; init; }
		public Boolean IsCamelCase { get; init; }

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
			else
				return XamlNode.GetNodeValue(builder, node);
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
