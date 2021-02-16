using System;
using System.IO;
using System.Xml;

namespace A2v10.System.Xaml
{
	public static class XamlServices
	{
		public static Object Parse(String xaml, XamlServicesOptions options = null)
		{
			return (new XamlReaderService()).ParseXml(xaml, options);
		}

		public static Object Load(Stream stream, XamlServicesOptions options = null)
		{
			return (new XamlReaderService()).Load(stream, options);
		}

		public static Object Load(XmlReader rdr, XamlServicesOptions options = null)
		{
			return (new XamlReaderService()).Load(rdr, options);
		}
	}
}
