using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace A2v10.System.Xaml
{
	public class XamlReaderService : IXamlReaderService
	{
		private readonly TypeDescriptorCache _typeDescriptorCache = new TypeDescriptorCache();

		public Object ParseXml(String xml, XamlServicesOptions options = null)
		{
			using var stringReader = new StringReader(xml);
			using var xmlrdr = XmlReader.Create(stringReader);
			return Load(xmlrdr, options);
		}

		public Object Load(Stream stream, XamlServicesOptions options = null)
		{
			var xaml = new XamlReader(XmlReader.Create(stream), _typeDescriptorCache, options);
			return xaml.Read();
		}

		public Object Load(XmlReader rdr, XamlServicesOptions options = null)
		{
			var xaml = new XamlReader(rdr, _typeDescriptorCache, options);
			return xaml.Read();
		}
	}
}
