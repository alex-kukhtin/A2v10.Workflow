
using System;
using System.IO;
using System.Xml;

namespace A2v10.System.Xaml
{
	public interface IXamlReaderService
	{
		Object ParseXml(String xml, XamlServicesOptions options = null);
		Object Load(Stream stream, XamlServicesOptions options = null);
		public Object Load(XmlReader rdr, XamlServicesOptions options = null);
	}
}
