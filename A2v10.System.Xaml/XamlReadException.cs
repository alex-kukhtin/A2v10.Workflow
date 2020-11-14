using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public sealed class XamlReadException : Exception
	{
		public XamlReadException(String message)
			:base(message)
		{

		}
	}
}
