
using System;
using System.Collections.Generic;

namespace A2v10.System.Xaml
{
	public class XamlNode
	{
		public String Name { get; init; }
		public String TextContent { get; set; }

		public Lazy<List<XamlNode>> Children = new Lazy<List<XamlNode>>();
		public readonly Dictionary<String, Object> Properties = new Dictionary<String, Object>();

		public Boolean HasChildren => Children.IsValueCreated && Children.Value.Count > 0;

		public Object GetPropertyValue(String propName, Type propType, NodeDefinition nodeDef)
		{
			if (Properties.TryGetValue(propName, out Object val))
			{
				if (val == null)
					return null;
				if (val.GetType() == (Nullable.GetUnderlyingType(propType) ?? propType))
					return val;
				throw new XamlReadException($"Invalid property type for '{propName}'. Expected: '{propType.Name}', actual: {val.GetType().Name}");
			}
			if (propType.IsEnum)
				return 0; // default value for enum
						  // try to get ContentProperty
			if (nodeDef.ContentProperty == propName)
				return GetContentProperty(nodeDef);
			return null;
		}

		public Object GetContentProperty(NodeDefinition nodeDef)
		{
			// set children as content
			if (!nodeDef.Properties.TryGetValue(nodeDef.ContentProperty, out PropDefinition propDef))
				throw new XamlReadException($"Property definition for property '{nodeDef.ContentProperty}' not found.");
			return GetPropertyValue(nodeDef, propDef);
		}

		private Object GetPropertyValue(NodeDefinition nodeDef, PropDefinition propDef)
		{ 
			if (propDef.Constructor == null)
			{
				// ContentProperty for text
				return TextContent;
			}

			if (!HasChildren)
				return null;
			if (propDef.AddMethod == null)
			{
				foreach (var c in Children.Value)
					return nodeDef.BuildNode(c); // return first chilren
			}
			var propObj = propDef.Constructor();
			foreach (var c in Children.Value)
			{
				propDef.AddMethod(propObj, nodeDef.BuildNode(c));
			}
			return propObj;
		}

		public void SetContent(String text)
		{
			TextContent = text;
		}

		public void AddChildren(XamlNode node, NodeBuilder builder)
		{
			if (node.Name.Contains("."))
			{
				// inner or attached
				var parts = node.Name.Split(".");
				if (parts.Length != 2)
					throw new XamlReadException($"Invalid attribute name '{node.Name}'");
				if (parts[0] == this.Name)
				{
					// nested property
					AddProperty(builder, parts[1], node);
				}
				else
				{
					throw new NotImplementedException("Attached property yet not implemented");
				}
			}
			else
			{
				Children.Value.Add(node);
			}
		}

		public void AddProperty(NodeBuilder builder, String name, XamlNode node)
		{
			var nd = builder.GetNodeDefinition(Name);
			Properties.Add(nd.MakeName(name), nd.BuildPropertyNode(builder, name, node));
		}

		public void AddProperty(NodeBuilder builder, String name, String value)
		{
			var nd = builder.GetNodeDefinition(Name);
			Properties.Add(nd.MakeName(name), nd.BuildProperty(name, value));
		}
	}
}
