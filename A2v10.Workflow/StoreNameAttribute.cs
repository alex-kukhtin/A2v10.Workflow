using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow
{
	public sealed class StoreNameAttribute : Attribute
	{
		public String Name { get; }

		public StoreNameAttribute(String name)
		{
			Name = name;
		}
	}
}
