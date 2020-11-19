using System;

namespace A2v10.System.Xaml
{

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ContentPropertyAttribute : Attribute
	{
		public String Name { get; }

		public ContentPropertyAttribute(String name)
		{
			Name = name;
		}
	}
}
