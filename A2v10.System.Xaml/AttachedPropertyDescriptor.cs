using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public record AttachedPropertyDescriptor
	{
		public Type PropertyType { get; set; }
		public Action<Object, Object> Lambda { get; init; }
		public Func<TypeConverter> TypeConverter { get; set; }
	}
}
