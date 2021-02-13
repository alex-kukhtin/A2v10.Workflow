using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Markup
{
	public class XamlProvideValueTarget : IProvideValueTarget
	{
		public object TargetObject { get; set; }

		public object TargetProperty { get; set; }
	}
}
