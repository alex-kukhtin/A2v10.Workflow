
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public record XamlExtensionElem(PropertyInfo PropertyInfo, MarkupExtension Element);

	public class XamlNode
	{
		public String Name { get; init; }
		public String TextContent { get; set; }
		public String ConstructorArgument => _ctorArgument;

		private String _ctorArgument;

		public Lazy<List<XamlNode>> Children = new Lazy<List<XamlNode>>();
		public readonly Dictionary<String, Object> Properties = new Dictionary<String, Object>();
		public readonly List<XamlExtensionElem> Extensions = new List<XamlExtensionElem>();

		public Boolean HasChildren => Children.IsValueCreated && Children.Value.Count > 0;

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
			{
				if (String.IsNullOrEmpty(TextContent))
					return null;

				// TextContent as Property
				if (propDef.TypeConverter != null) {
					var conv = propDef.TypeConverter();
					if (conv.CanConvertFrom(typeof(String)))
						return conv.ConvertFrom(null, CultureInfo.InvariantCulture, TextContent);
				}
				return null;
			}
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
			var propName = builder.QualifyPropertyName(name);
			var nd = builder.GetNodeDefinition(Name);
			if (value != null && value.StartsWith("{") && value.EndsWith("}") && builder.EnableMarkupExtensions)
				Extensions.Add(new XamlExtensionElem(nd.GetPropertyInfo(nd.MakeName(name)), builder.ParseExtension(value)));
			else if (propName.Contains('.'))
			{
				; // attached properties
				int z = 55;
			} 
			else
				Properties.Add(nd.MakeName(name), nd.BuildProperty(propName, value));
		}

		public void AddConstructorArgument(String value)
		{
			_ctorArgument = value;
		}
	}
}
