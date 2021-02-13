using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace A2v10.System.Xaml.Tests.Mock
{
	[DefaultProperty("Name")]
	public class BindCmd : BindBase
	{
		public String Name { get; set; }
		public String Argument { get; set; }

		public BindCmd()
		{

		}

		public BindCmd(String name)
		{
			Name = name;
		}
	}
}
