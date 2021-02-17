using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public record CollectionLambdas
	{
		public Action<Object, Object> AddCollection;
		public Action<Object, String, Object> AddDictionary;
	}

	public static class LambdaHelper
	{
		public static CollectionLambdas AddCollectionMethods(Type propType)
		{
			var result = new CollectionLambdas();

			var mtdAdd = propType.GetMethod("Add");
			if (mtdAdd == null)
				return result;
			var inst = Expression.Parameter(typeof(Object));
			var argPrm = Expression.Parameter(typeof(Object));

			var args = mtdAdd.GetParameters();
			if (args.Length == 1)
			{
				var argType = args[0].ParameterType;

				result.AddCollection = Expression.Lambda<Action<Object, Object>>(
					Expression.Call(
						Expression.Convert(inst, propType),
						mtdAdd,
						Expression.Convert(
							argPrm,
							argType
						)
					),
					inst,
					argPrm
				).Compile();
			} 
			else if (args.Length == 2)
			{
				// .Add(key, value);
				var argType = args[1].ParameterType; // value
				var argKey = Expression.Parameter(typeof(String));

				result.AddDictionary = Expression.Lambda<Action<Object, String, Object>>(
					Expression.Call(
						Expression.Convert(inst, propType),
						mtdAdd,
						argKey,
						Expression.Convert(
							argPrm,
							argType
						)
					),
					inst,
					argKey,
					argPrm
				).Compile();
			}
			return result;
		}
	}
}
