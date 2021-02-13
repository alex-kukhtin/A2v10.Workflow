using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public interface IProvideValueTarget
	{
		Object TargetObject { get; }
		Object TargetProperty { get; }
	}
}
