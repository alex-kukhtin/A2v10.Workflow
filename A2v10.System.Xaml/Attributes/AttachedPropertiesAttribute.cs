using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AttachedPropertiesAttribute : Attribute
	{
		public String List { get; }

		public AttachedPropertiesAttribute(String list)
		{
			List = list;
		}
	}
}
