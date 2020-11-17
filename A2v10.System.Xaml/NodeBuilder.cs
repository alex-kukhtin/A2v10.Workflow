using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{

	public record PropDefinition
	{
		public Type Type { get; init; }
		public Func<Object> Constructor { get; init; }
		public Action<Object, Object> AddMethod { get; init; }
		public Func<String, Object> EnumConvert { get; init; }
	}

	public record NodeDefinition
	{
		public Func<XamlNode, Object> Lambda { get; init; }
		public Dictionary<String, PropDefinition> Properties { get; init; }

		public Object BuildProperty(String name, Object value)
		{
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found");
			if (value == null)
				return null;
			if (value.GetType() == propDef.Type)
				return value;
			if (propDef.EnumConvert != null)
				return propDef.EnumConvert(value.ToString());

			throw new NotImplementedException($"Property {name}");
		}

		public Object BuildPropertyNode(NodeBuilder builder, String name, XamlNode node)
		{
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found");
			if (node == null)
				return null;
			var obj = propDef.Constructor();
			if (propDef.AddMethod != null)
			{
				if (node.Children.IsValueCreated)
					foreach (var nd in node.Children.Value)
						propDef.AddMethod(obj, builder.BuildNode(nd));
			}
			return obj;
		}
	}

	public class NamespaceDefinition
	{
		public String Namespace { get; init; }
		public Assembly Assembly { get; init; }
	}

	public record ClassNamePair
	{
		public String Prefix { get; init; }
		public String Namespace { get; init; }
		public String ClassName { get; set; }
	}

	public class NodeBuilder
	{
		private static readonly MethodInfo _getNodePropertyValue = typeof(XamlNode).GetMethod("GetPropertyValue", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _enumParse = typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(String)});

		private static readonly ConcurrentDictionary<ClassNamePair, NodeDefinition> _typeCache = new ConcurrentDictionary<ClassNamePair, NodeDefinition>();

		private readonly Dictionary<String, NamespaceDefinition> _namespaces = new Dictionary<String, NamespaceDefinition>();


		private static readonly Regex _namespaceRegEx = new Regex(@"^\s*clr-namespace\s*:\s*([\w\.]+)\s*;\s*assembly\s*=\s*([\w\.]+)\s*$", RegexOptions.Compiled);

		public void AddNamespace(String prefix, String value)
		{
			if (value == "http://schemas.microsoft.com/winfx/2006/xaml")
			{
				// xaml namespace for x:Key, etc
				return;
			}
			var match = _namespaceRegEx.Match(value);
			if (match.Groups.Count == 3)
			{
				var assemblyName = match.Groups[2].Value.Trim();
				var nameSpace = match.Groups[1].Value.Trim();
				var nsd = new NamespaceDefinition()
				{
					Namespace = nameSpace,
					Assembly = Assembly.Load(assemblyName)
				};
				_namespaces.Add(prefix, nsd);
			}
		}

		/* Returns:
			Func<XamlNode, Object> Lambda = (node) => new NodeClass() {
				Prop1 = node.GetPropertyValue("Prop1", propType), 
				Prop2 = node.GetPropertyValue("Prop2", propType)
			};
		*/

		private static PropDefinition BuildPropertyDefinition(Type propType)
		{
			Func<Object> ctor = null;
			Action<Object, Object> addMethod = null;
			Func<String, Object> enumConvert = null;
			if (propType.IsEnum)
			{
				var parsePrm = Expression.Parameter(typeof(String));
				enumConvert = Expression.Lambda<Func<String, Object>>(
					Expression.Call(_enumParse, 
						Expression.Constant(propType),
						parsePrm
					),
					parsePrm
				).Compile();
			}
			else if (!propType.IsPrimitive && propType != typeof(String))
			{
				var propCtor = propType.GetConstructor(Array.Empty<Type>());
				ctor = Expression.Lambda<Func<Object>>(Expression.New(propCtor)).Compile();
			}
			var mtdAdd = propType.GetMethod("Add");
			if (mtdAdd != null)
			{
				var args = mtdAdd.GetParameters();
				if (args.Length != 1)
					throw new XamlReadException("Invalid argument count for Add parameter");
				var argType = args[0].ParameterType;

				var argPrm = Expression.Parameter(typeof(Object));
				var inst = Expression.Parameter(typeof(Object));

				addMethod = Expression.Lambda<Action<Object, Object>>(
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

			return new PropDefinition()
			{
				Type = propType,
				Constructor = ctor,
				AddMethod = addMethod,
				EnumConvert = enumConvert
			};
		}

		private NodeDefinition BuildNodeDefinition(ClassNamePair namePair)
		{
			if (!_namespaces.TryGetValue(namePair.Prefix, out NamespaceDefinition nsd))
				throw new XamlReadException($"Namespace {namePair.Namespace} not found");
			var nodeType = nsd.Assembly.GetType($"{namePair.Namespace}.{namePair.ClassName}");
			var param = Expression.Parameter(typeof(XamlNode));
			var ctor = nodeType.GetConstructor(Array.Empty<Type>());
			var props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var inits = new List<MemberAssignment>();
			var propDefs = new Dictionary<String, PropDefinition>();
			foreach (var prop in props)
			{
				propDefs.Add(prop.Name, BuildPropertyDefinition(prop.PropertyType));

				inits.Add(Expression.Bind(prop,
					Expression.Convert(
						Expression.Call(
							param,
							_getNodePropertyValue,
							Expression.Constant(prop.Name),
							Expression.Constant(prop.PropertyType)
						),
						prop.PropertyType)
				));
			}
			var newExpr = Expression.New(ctor);
			var expr = Expression.MemberInit(newExpr, inits);
			// lambda
			Func<XamlNode, Object> lambda = Expression.Lambda<Func<XamlNode, Object>>(expr, param).Compile();
			// props

			return new NodeDefinition()
			{
				Lambda = lambda,
				Properties = propDefs
			};
		}

		public NodeDefinition GetNodeDefinition(String typeName)
		{
			//GetNode
			ClassNamePair className = null;
			if (typeName.Contains(":"))
			{
				// type with namespace
			}
			else
			{
				if (_namespaces.TryGetValue(String.Empty, out NamespaceDefinition nsd))
					className = new ClassNamePair()
					{
						Prefix = String.Empty,
						Namespace = nsd.Namespace,
						ClassName = typeName
					};
				else
					throw new XamlReadException("Default namespace not found");
			}
			return _typeCache.GetOrAdd(className, BuildNodeDefinition);
		}

		public Object BuildNode(XamlNode node)
		{
			var nd = GetNodeDefinition(node.Name);
			return nd.Lambda(node);
		}
	}
}
