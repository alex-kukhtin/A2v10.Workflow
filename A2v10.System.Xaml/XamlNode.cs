
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace A2v10.System.Xaml
{
	public record XamlExtenesionElem(PropertyInfo PropertyInfo, MarkupExtension Element);

	public class XamlNode
	{
		public String Name { get; init; }
		public String TextContent { get; set; }

		private String _ctorArgument;

		public Lazy<List<XamlNode>> Children = new Lazy<List<XamlNode>>();
		public readonly Dictionary<String, Object> Properties = new Dictionary<String, Object>();
		public readonly List<XamlExtenesionElem> Extensions = new List<XamlExtenesionElem>();

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
			if (_ctorArgument != null && propName == nodeDef.DefaultProperty)
			{
				if (propType.IsEnum)
				{
					if (Enum.TryParse(propType, _ctorArgument, out object result))
						return result;
					throw new XamlReadException($"Unable to convert '{_ctorArgument}' to type '{propType.Name}'");
				}
				return Convert.ChangeType(_ctorArgument, propType);
			}
			if (propType.IsEnum)
				return 0; // default value for enum
						  // try to get ContentProperty
			if (nodeDef.ContentProperty == propName)
				return GetContentProperty(nodeDef);
			return null;
		}

		public Object InitCreatedElement(Object elem)
		{
			if (Extensions.Count > 0)
			{
				var provider = new XamlServiceProvider();
				var valueTarget = new XamlProvideValueTarget();
				provider.AddService<IProvideValueTarget>(valueTarget);

				valueTarget.TargetObject = elem;
				foreach (var ext in Extensions)
				{
					valueTarget.TargetProperty = ext.PropertyInfo;
					ext.Element.ProvideValue(provider);
				}
			}
			if (elem is ISupportInitialize init)
			{
				init.BeginInit();
				init.EndInit();
			}
			return elem;
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
			var propName = builder.QualifyPropertyName(name);
			var nd = builder.GetNodeDefinition(Name);
			if (value != null && value.StartsWith("{") && value.EndsWith("}") && builder.EnableMarkupExtensions)
				Extensions.Add(new XamlExtenesionElem(nd.GetPropertyInfo(nd.MakeName(name)), builder.ParseExtension(value)));
			else
				Properties.Add(nd.MakeName(name), nd.BuildProperty(propName, value));
		}

		public void AddConstructorArgument(String value)
		{
			_ctorArgument = value;
		}
	}
}
