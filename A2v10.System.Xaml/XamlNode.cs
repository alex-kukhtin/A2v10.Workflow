
using System;
using System.Collections.Generic;

namespace A2v10.System.Xaml
{
	public class XamlNode
	{
		public String Name { get; init; }

		public Lazy<List<XamlNode>> Children = new Lazy<List<XamlNode>>();
		public readonly Dictionary<String, Object> Properties = new Dictionary<String, Object>();

		public Object GetPropertyValue(String propName, Type propType)
		{
			if (Properties.TryGetValue(propName, out Object val))
			{
				if (val == null)
					return null;
				if (val.GetType() == propType)
					return val;
				throw new XamlReadException($"Invalid property type for '{propName}'. Expected: '{propType.Name}', actual: {val.GetType().Name}");
			}
			if (propType.IsEnum)
				return 0; // default value for enum
			return null;
		}

		public void SetContent(String text)
		{
			Console.WriteLine($"Set content: {text}");
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
				Console.WriteLine($"AddChildren: {node.Name} to {this.Name}");
				Children.Value.Add(node);
			}
		}

		public void AddProperty(NodeBuilder builder, String name, XamlNode node)
		{
			var nd = builder.GetNodeDefinition(Name);
			Properties.Add(name, nd.BuildPropertyNode(builder, name, node));
		}

		public void AddProperty(NodeBuilder builder, String name, String value)
		{
			var nd = builder.GetNodeDefinition(Name);
			Properties.Add(name, nd.BuildProperty(name, value));
		}

		public void AddAttribute(NodeBuilder builder, String name, String value)
		{
			if (name.StartsWith("xmlns"))
				AddNamespace(builder, name, value);
			else
				AddProperty(builder, name, value);
		}

		static void AddNamespace(NodeBuilder builder, String name, String value)
		{
			// xmlns:x;
			var prefix = name[5..];
			if (prefix.StartsWith(':'))
				prefix = prefix[1..] + ":";
			builder.AddNamespace(prefix, value);
		}
	}
}
