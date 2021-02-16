using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public class TypeDescriptorCache
	{
		private readonly ConcurrentDictionary<ClassNamePair, TypeDescriptor> _descriptorCache = new ConcurrentDictionary<ClassNamePair, TypeDescriptor>();

		public TypeDescriptor GetOrAdd(ClassNamePair key, Func<ClassNamePair, TypeDescriptor> valueFactory)
		{
			return _descriptorCache.GetOrAdd(key, valueFactory);
		}
	}
}
