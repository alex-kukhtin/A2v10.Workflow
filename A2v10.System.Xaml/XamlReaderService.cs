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

		public virtual XamlServicesOptions Options { get; set; }


		public Object ParseXml(String xml)
		{
			using var stringReader = new StringReader(xml);
			using var xmlrdr = XmlReader.Create(stringReader);
			return Load(xmlrdr);
		}

		public Object Load(Stream stream, Uri baseUri = null)
		{
			var xaml = new XamlReader(XmlReader.Create(stream), baseUri, _typeDescriptorCache, Options);
			Options?.OnCreateReader?.Invoke(xaml);
			return xaml.Read();
		}

		public Object Load(XmlReader rdr, Uri baseUri = null)
		{
			var xaml = new XamlReader(rdr, baseUri, _typeDescriptorCache, Options);
			Options?.OnCreateReader?.Invoke(xaml);
			return xaml.Read();
		}
	}
}
