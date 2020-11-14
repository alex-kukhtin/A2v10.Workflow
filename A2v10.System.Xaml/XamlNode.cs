using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public record XamlAttribute
	{
		public String Name { get; init; }
		public String Value { get; init; }
	}

	public record XamlProperty
	{
		public String Name { get; init; }
	}

	public record PlainProperty : XamlProperty
	{
		public String Value { get; init; }
	}

	public record NodeProperty : XamlProperty
	{
		public XamlNode Node { get; init; }
	}

	public class XamlNode
	{
		public String Name { get; init; }

		public Lazy<List<XamlNode>> Children = new Lazy<List<XamlNode>>();
		public Dictionary<String, Object> Properties = new Dictionary<String, Object>();

		public Object GetPropertyValue(String propName)
		{
			if (Properties.TryGetValue(propName, out Object val))
				return val;
			return null;
		}

		public void AddChildren(XamlNode node)
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
					AddProperty(parts[1], node);
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

		public void AddProperty(String name, XamlNode node)
		{
			Properties.Add(name, null);
		}

		public void AddAttribute(String name, String value)
		{
			if (name.StartsWith("xmlns"))
				AddNamespace(name, value);
			else
				Properties.Add(name, value);
		}

		void AddNamespace(String name, String value)
		{
			Console.WriteLine($"namespace {name}='{value}' to {this.Name}");
		}

		public String ToCSharpCode(int level = 0)
		{
			return null;
		}
	}
}
