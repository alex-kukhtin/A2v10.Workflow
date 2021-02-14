using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public class XamlServiceProvider : IServiceProvider
	{
		private readonly Dictionary<Type, Object> _services = new Dictionary<Type, Object>();
		private readonly XamlProvideValueTarget _provideValueTarget = new XamlProvideValueTarget();
		private readonly XamlRootObjectProvider _rootObjectProvider = new XamlRootObjectProvider();


		public XamlServiceProvider()
		{
			AddService<IProvideValueTarget>(_provideValueTarget);
			AddService<IRootObjectProvider>(_rootObjectProvider);
		}

		public void AddService<T>(Object service)
		{
			_services.Add(typeof(T), service);
		}

		public void AddService(Type serviceType, Object service)
		{
			_services.Add(serviceType, service);
		}


		public object GetService(Type serviceType)
		{
			if (_services.TryGetValue(serviceType, out Object service))
				return service;
			return null;
		}

		public XamlProvideValueTarget ProvideValueTarget => _provideValueTarget;

		public void SetRoot(Object root)
		{
			_rootObjectProvider.RootObject = root;
		}

	}
}
