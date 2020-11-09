
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests.Mocks
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class CallbackItem234
	{
		public String Ref;
		public String CallbackName;

		// for InMemory storage
		public IActivity Activity;
		public MethodInfo Callback;
		
		public void Restore()
		{
			// find Callback by Name
		}
	}

	public class ActivityStorageMock234 : IActivityStorage
	{
		Dictionary<String, Object> _storage = new Dictionary<String, Object>();

		public T Get<T>(String name)
		{
			throw new NotImplementedException();
		}

		public ExecutingAction GetCallback(String name)
		{
			throw new NotImplementedException();
		}

		public void Set<T>(String name, T value)
		{
			if (_storage.ContainsKey(name))
				_storage[name] = value;
			else
				_storage.Add(name, value);
		}

		public void SetCallback(String name, ExecutingAction callback)
		{
			if (!(callback.Target is IActivity activityTarget))
				throw new ArgumentOutOfRangeException("callback.Target must be an IActivity");
			var refer = activityTarget.Ref;

			var custAttr = callback.Method.GetCustomAttributes(inherit:true)
				?.FirstOrDefault(attr => attr is StoreNameAttribute);

			if (custAttr == null)
				throw new ArgumentOutOfRangeException("callback.Method has no StoreName attribute");
			var callbackName = (custAttr as StoreNameAttribute).Name;

			var cb = new CallbackItem() 
			{ 
				Ref = refer, 
				CallbackName = callbackName
			};

			Set(name, cb);
		}
	}
}
