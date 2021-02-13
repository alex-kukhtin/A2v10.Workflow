using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Markup
{
	public interface IProvideValueTarget
	{
		Object TargetObject { get; }
		Object TargetProperty { get; }
	}
}
