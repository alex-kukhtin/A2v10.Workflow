using System;
using System.Collections.Generic;
using System.Xml;

namespace A2v10.System.Xaml
{
	public class XamlReader
	{
		private readonly XamlNode _root = new XamlNode() { Name = "Root" };
		private readonly XmlReader _rdr;

		private readonly Stack<XamlNode> _elemStack = new Stack<XamlNode>();

		public XamlReader(XmlReader rdr)
		{
			_rdr = rdr;
			_elemStack.Push(_root);
		}

		public Object Read()
		{
			var nodeBuilder = new NodeBuilder();
			while (_rdr.Read())
			{
				if (_rdr.NodeType == XmlNodeType.Comment)
					continue;
				ReadNode(nodeBuilder);
			}
			if (!_root.Children.IsValueCreated)
				return null;
			if (_root.Children.Value.Count == 0)
				return 0;
			else if (_root.Children.Value.Count > 1)
				throw new XamlReadException("Invalid Xaml structure");
			var r = _root.Children.Value[0];
			return nodeBuilder.BuildNode(r);
		}

		void ReadNode(NodeBuilder builder)
		{
			switch (_rdr.NodeType)
			{
				case XmlNodeType.Element:
					{
						var node = new XamlNode()
						{
							Name = _rdr.Name
						};
						StartNode(node);
						if (_rdr.IsEmptyElement)
							EndNode(builder);
						ReadAttributes(node, builder);
					}
					break;
				case XmlNodeType.EndElement:
					EndNode(builder);
					break;
				case XmlNodeType.Text:
					AddContent();
					break;
			}
		}

		void ReadAttributes(XamlNode node, NodeBuilder builder)
		{
			for (var i = 0; i < _rdr.AttributeCount; i++)
			{
				_rdr.MoveToAttribute(i);
				node.AddAttribute(builder, _rdr.Name, _rdr.Value);
			}
		}

		void StartNode(XamlNode node)
		{
			_elemStack.Push(node);
		}

		void AddContent()
		{
			var node = _elemStack.Peek();
			node.SetContent(_rdr.Value?.Trim());
		}

		void EndNode(NodeBuilder builder)
		{
			var ch = _elemStack.Pop();
			var parent = _elemStack.Peek();
			parent.AddChildren(ch, builder);
		}
	}
}
