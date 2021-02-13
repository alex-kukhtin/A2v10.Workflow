using System;

namespace A2v10.System.Xaml
{

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		public String Name { get; }

		public DefaultPropertyAttribute(String name)
		{
			Name = name;
		}
	}
}
