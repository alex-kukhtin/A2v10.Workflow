using System;
using System.IO;
using System.Xml;

namespace A2v10.System.Xaml
{
	public static class XamlServices
	{
		public static Object Parse(String xaml, XamlServicesOptions options = null)
		{
			using var stringReader = new StringReader(xaml);
			using var xmlrdr = XmlReader.Create(stringReader);
			return Load(xmlrdr, options);
		}

		static Object Load(XmlReader rdr, XamlServicesOptions options = null)
		{
			var xaml = new XamlReader(rdr, options);
			return xaml.Read();
		}
	}
}
