using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public class XamlProvideValueTarget : IProvideValueTarget
	{
		public object TargetObject { get; set; }

		public object TargetProperty { get; set; }
	}
}
