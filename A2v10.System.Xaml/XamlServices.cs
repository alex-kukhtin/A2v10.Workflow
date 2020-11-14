using System;
using System.IO;
using System.Xml;

namespace A2v10.System.Xaml
{
	public static class XamlServices
	{
		public static Object Parse(String xaml)
		{
			using var stringReader = new StringReader(xaml);
			using var xmlrdr = XmlReader.Create(stringReader);
			return Load(xmlrdr);
		}

		public static Object Load(XmlReader rdr)
		{
			var xaml = new XamlReader(rdr);
			return xaml.Read();
		}
	}
}
