using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public class ExtensionParser
	{
		public static XamlNode Parse(NodeBuilder builder, String text)
		{
			return (new ExtensionParser(builder, text)).Parse();
		}

		private readonly NodeBuilder _builder;
		private readonly String _text;
		private readonly Int32 _len;
		private Int32 _pos;
		private Char _ch;
		private XamlNode _node;

		private ExtensionParser(NodeBuilder builder, String text)
		{
			_builder = builder;
			_text = text;
			_len = _text.Length;
			SetPosition(0);
			NextToken();
			_node = new XamlNode();
		}

		XamlNode Parse()
		{
			_node = new XamlNode() { Name = "BindCmd" };

			//_node.AddProperty(_builder, "Name", "Execute");
			_node.AddConstructorArgument("Execute");

			// RECURSIVE!!!!
			//xamlNode.AddProperty(_builder, "Argument", "{BindCmd Name='Value'}");

			return _node;
		}

		void SetPosition(Int32 pos)
		{
			_pos = 0;
			_ch = _pos < _len ? _text[_pos] : '\0';
		}

		void NextChar()
		{

		}


		void NextToken()
		{
			while (Char.IsWhiteSpace(_ch)) NextChar();
		}
	}
}
