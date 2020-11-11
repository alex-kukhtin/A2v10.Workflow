using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

using System.Collections.Generic;
using System.Text;
using A2v10.Workflow.Interfaces;
using System.Dynamic;
using System.Transactions;

namespace A2v10.Workflow
{
	using ResumeAction = Func<IExecutionContext, String, Object, ValueTask>;
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

	public class CallbackItem
	{
		public String Ref;
		public String CallbackName;

		public static CallbackItem FromExpando(ExpandoObject eobj)
		{
			if (eobj == null)
				return null;
			var cb = new CallbackItem() {
				Ref = eobj.Get<String>(nameof(Ref)),
				CallbackName = eobj.Get<String>(nameof(CallbackName))
			};
			return cb;
		}

		public static ExpandoObject CreateFrom(Delegate callback)
		{
			if (callback.Target is not IActivity activityTarget)
				throw new ArgumentException("callback.Target must be an IActivity");

			var refer = activityTarget.Ref;

			var custAttr = callback.Method.GetCustomAttributes(inherit: true)
				?.FirstOrDefault(attr => attr is StoreNameAttribute);

			if (custAttr == null)
				throw new ArgumentException("callback.Method has no StoreName attribute");

			var callbackName = (custAttr as StoreNameAttribute).Name;

			var cb = new CallbackItem()
			{
				Ref = refer,
				CallbackName = callbackName
			};
			return cb.ToExpando();
		}

		private ExpandoObject ToExpando()
		{
			var eo = new ExpandoObject();
			eo.Set(nameof(Ref), Ref);
			eo.Set(nameof(CallbackName), CallbackName);
			return eo;
		}

		private MethodInfo GetMethod(IActivity activity)
		{
			foreach (var mi in activity.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				var custAttr = mi.GetCustomAttributes(inherit: true)
					?.FirstOrDefault(attr => attr is StoreNameAttribute);
				if (custAttr != null)
				{
					var callbackName = (custAttr as StoreNameAttribute).Name;
					if (callbackName == CallbackName)
					{
						return mi;
					}
				}
			}
			throw new WorkflowExecption($"Method '{CallbackName}' for activity '{Ref}' not found");
		}

		public ExecutingAction ToCallback(IActivity activity)
		{
			var mi = GetMethod(activity);
			return Delegate.CreateDelegate(typeof(ExecutingAction), activity, mi) as ExecutingAction;
		}

		public ResumeAction ToBookmark(IActivity activity)
		{
			var mi = GetMethod(activity);
			return Delegate.CreateDelegate(typeof(ResumeAction), activity, mi) as ResumeAction;
		}
	}

}
