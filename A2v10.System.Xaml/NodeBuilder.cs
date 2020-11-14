using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public class NodeBuilder
	{
		private readonly MethodInfo _getNodePropertyValue;
		private readonly Assembly _assembly;

		public NodeBuilder()
		{
			_getNodePropertyValue = typeof(XamlNode).GetMethod("GetPropertyValue", BindingFlags.Public | BindingFlags.Instance);
			_assembly = Assembly.Load("A2v10.Workflow");
		}


		/* Returns:
			Func<XamlNode, Object> func = (node) => new NodeClass() {
				Prop1 = node.GetPropertyValue("Prop1"), 
				Prop2 = node.GetPropertyValue("Prop2")
			};
		*/
		public Object BuildNode(XamlNode node)
		{
			var className = $"A2v10.Workflow.{node.Name}";
			var nodeType = _assembly.GetType(className);
			var param = Expression.Parameter(typeof(XamlNode));
			var ctor = nodeType.GetConstructor(new Type[0]);
			var props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var inits = new List<MemberAssignment>();
			foreach (var prop in props) {
				inits.Add(Expression.Bind(prop,
					Expression.Convert(
						Expression.Call(
							param, _getNodePropertyValue, Expression.Constant(prop.Name)
						),
						prop.PropertyType)
				));
			}
			var newExpr = Expression.New(ctor);
			var expr = Expression.MemberInit(newExpr, inits);
			// lambda
			Func<XamlNode, Object> d = Expression.Lambda<Func<XamlNode, Object>>(expr, param).Compile();


			return d(node);
		}
	}
}
