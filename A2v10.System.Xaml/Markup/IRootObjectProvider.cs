using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Markup
{
	public interface IRootObjectProvider
	{
		Object RootObject { get; }
	}
}
