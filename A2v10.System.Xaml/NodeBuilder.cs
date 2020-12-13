using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace A2v10.System.Xaml
{

	public record PropDefinition
	{
		public Type Type { get; init; }
		public Func<Object> Constructor { get; init; }
		public Action<Object, Object> AddMethod { get; init; }
		public Func<String, Object> EnumConvert { get; init; }
		public Func<String, Object> ScalarConvert { get; init; }
	}

	public record NodeDefinition
	{
		public String ClassName { get; init; }
		public Func<XamlNode, NodeDefinition, Object> Lambda { get; init; }
		public Dictionary<String, PropDefinition> Properties { get; init; }
		public String ContentProperty { get; init; }
		public Func<XamlNode, Object> BuildNode { get; init; }
		public Type NodeType { get; init; }
		public Boolean IsCamelCase { get; init; }

		public Object BuildProperty(String name, Object value)
		{
			name = MakeName(name);
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found in type {ClassName}");
			if (value == null)
				return null;
			if (value.GetType() == propDef.Type)
				return value;
			if (propDef.EnumConvert != null)
				return propDef.EnumConvert(value.ToString());
			if (propDef.ScalarConvert != null)
				return propDef.ScalarConvert(value.ToString());
			throw new NotImplementedException($"Property {name}");
		}

		public Object BuildPropertyNode(NodeBuilder builder, String name, XamlNode node)
		{
			if (!Properties.TryGetValue(name, out PropDefinition propDef))
				throw new XamlReadException($"Property {name} not found");
			if (node == null)
				return null;
			if (propDef.Constructor != null)
			{
				var obj = propDef.Constructor();
				if (propDef.AddMethod != null)
				{
					if (node.HasChildren)
						foreach (var nd in node.Children.Value)
							propDef.AddMethod(obj, builder.BuildNode(nd));
				}
				return obj;
			}
			else if (propDef.EnumConvert != null)
				return propDef.EnumConvert(node.TextContent);
			else if (propDef.ScalarConvert != null)
			{
				var nval = GetNodeValue(builder, node);
				if (nval.GetType() == propDef.Type)
					return nval;
				return propDef.ScalarConvert(nval?.ToString());
			}
			else
				return GetNodeValue(builder, node);
		}

		static Object GetNodeValue(NodeBuilder builder, XamlNode node)
		{
			if (!node.HasChildren)
				return node.TextContent;
			if (node.Name.Contains('.'))
			{
				var ch = node.Children.Value[0];
				return builder.BuildNode(ch);
			}
			return node.TextContent;
		}

		public String MakeName(String name)
		{
			if (IsCamelCase)
			{
				// source: camelCase
				// code: PascalCase
				return name.ToPascalCase();
			}
			return name;
		}
	}

	public class NamespaceDefinition
	{
		public String Namespace { get; init; }
		public Assembly Assembly { get; init; }
		public Boolean IsCamelCase { get; init; }
	}

	public record ClassNamePair
	{
		public String Prefix { get; init; }
		public String Namespace { get; init; }
		public String ClassName { get; init; }
		public Boolean IsCamelCase { get; init; }
	}

	public record NamespaceDef(String Name, Boolean IsCamelCase, String Namespace, String Assembly);

	public class NodeBuilder
	{
		private static readonly MethodInfo _getNodePropertyValue =
			typeof(XamlNode).GetMethod("GetPropertyValue", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _enumParse =
			typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(String) });
		private static readonly MethodInfo _convertChangeType =
			typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(Object), typeof(Type), typeof(CultureInfo) });

		private static readonly ConcurrentDictionary<ClassNamePair, NodeDefinition> _typeCache = new ConcurrentDictionary<ClassNamePair, NodeDefinition>();

		private readonly Dictionary<String, NamespaceDefinition> _namespaces = new Dictionary<String, NamespaceDefinition>();

		private readonly NamespaceDef[] BPMNNamespaces = new NamespaceDef[] { 
			new NamespaceDef("http://www.omg.org/spec/bpmn/20100524/model", true, "A2v10.Workflow.Bpmn", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/bpmn/20100524/di", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/dd/20100524/di", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow"),
			new NamespaceDef("http://www.omg.org/spec/dd/20100524/dc", false, "A2v10.Workflow.Bpmn.Diagram", "A2v10.Workflow")
		};

		NamespaceDef IsBpmnNamespace(String value)
		{
			value = value.ToLowerInvariant();
			return BPMNNamespaces.FirstOrDefault(x => x.Name == value);
		}

		private static readonly Regex _namespaceRegEx = new Regex(@"^\s*clr-namespace\s*:\s*([\w\.]+)\s*;\s*assembly\s*=\s*([\w\.]+)\s*$", RegexOptions.Compiled);

		public void AddNamespace(String prefix, String value)
		{
			if (value == "http://schemas.microsoft.com/winfx/2006/xaml")
			{
				// xaml namespace for x:Key, etc
				return;
			}
			var nsddef = IsBpmnNamespace(value);
			if (nsddef != null)
			{
				var nsd = new NamespaceDefinition()
				{
					Namespace = nsddef.Namespace,
					Assembly = Assembly.Load(nsddef.Assembly),
					IsCamelCase = nsddef.IsCamelCase
				};
				_namespaces.Add(prefix, nsd);
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

		private static PropDefinition BuildPropertyDefinition(Type propType)
		{
			Func<Object> ctor = null;
			Action<Object, Object> addMethod = null;
			Func<String, Object> enumConvert = null;
			Func<String, Object> scalarConvert = null;
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
			else if (propType.IsPrimitive || propType.IsValueType)
			{
				var invCulture = typeof(CultureInfo).GetProperty("InvariantCulture");
				var convertPrm = Expression.Parameter(typeof(Object));
				var convertType = Nullable.GetUnderlyingType(propType) ?? propType;
				scalarConvert = Expression.Lambda<Func<String, Object>>(
					Expression.Call(_convertChangeType,
						convertPrm,
						Expression.Constant(convertType),
						Expression.Property(null, invCulture)
					),
					convertPrm
				).Compile();
			}
			else if (propType != typeof(String))
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
				EnumConvert = enumConvert,
				ScalarConvert = scalarConvert
			};
		}

		/* Returns:
			Func<XamlNode, Object> Lambda = (node, nodeDef) => new NodeClass() {
				Prop1 = node.GetPropertyValue("Prop1", propType, nodeDef), 
				Prop2 = node.GetPropertyValue("Prop2", propType, nodeDef)
			};
		*/
		private NodeDefinition BuildNodeDefinition(ClassNamePair namePair)
		{
			if (!_namespaces.TryGetValue(namePair.Prefix, out NamespaceDefinition nsd))
				throw new XamlReadException($"Namespace {namePair.Namespace} not found");
			var nodeType = nsd.Assembly.GetType($"{namePair.Namespace}.{namePair.ClassName}");
			if (nodeType == null)
				throw new XamlReadException($"Class {namePair.Namespace}.{namePair.ClassName} not found");
			var param = Expression.Parameter(typeof(XamlNode));
			var nodeDef = Expression.Parameter(typeof(NodeDefinition));
			var ctor = nodeType.GetConstructor(Array.Empty<Type>());
			var props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var inits = new List<MemberAssignment>();
			var propDefs = new Dictionary<String, PropDefinition>();
			foreach (var prop in props.Where(p => p.CanWrite))
			{
				propDefs.Add(prop.Name, BuildPropertyDefinition(prop.PropertyType));

				// (T) GetPropertyValue(propName, propType, nodeDef) ?? default<T>
				inits.Add(Expression.Bind(prop,
					Expression.Convert(
						Expression.Coalesce(
							Expression.Call(
								param,
								_getNodePropertyValue,
								Expression.Constant(prop.Name),
								Expression.Constant(prop.PropertyType),
								nodeDef
							),
							Expression.Default(prop.PropertyType)
						),
						prop.PropertyType)
				));
			}

			Func<XamlNode, NodeDefinition, Object> lambda = null;
			if (ctor != null)
			{
				var newExpr = Expression.New(ctor);
				var expr = Expression.MemberInit(newExpr, inits);

				// lambda
				lambda = Expression.Lambda<Func<XamlNode, NodeDefinition, Object>>(expr, param, nodeDef).Compile();
			}

			return new NodeDefinition()
			{
				ClassName = namePair.ClassName,
				Lambda = lambda,
				Properties = propDefs,
				ContentProperty = nodeType.GetCustomAttribute<ContentPropertyAttribute>()?.Name,
				BuildNode = this.BuildNode,
				NodeType = nodeType,
				IsCamelCase = namePair.IsCamelCase
			};
		}

		public NodeDefinition GetNodeDefinition(String typeName)
		{
			//GetNode
			String nsKey = String.Empty;
			if (typeName.Contains(":"))
			{
				// type with namespace
				var pair = typeName.Split(':');
				nsKey = pair[0];
				typeName = pair[1];
			}
			if (_namespaces.TryGetValue(nsKey, out NamespaceDefinition nsd))
			{
				if (nsd.IsCamelCase)
				{
					// file: camelCase
					// code: PascalCase
					typeName = typeName.ToPascalCase();
				}
				var className = new ClassNamePair()
				{
					Prefix = nsKey,
					Namespace = nsd.Namespace,
					ClassName = typeName,
					IsCamelCase = nsd.IsCamelCase
				};
				return _typeCache.GetOrAdd(className, BuildNodeDefinition);
			}
			else
				throw new XamlReadException($"Namespace '{nsKey}' not found");
		}

		public Object BuildNode(XamlNode node)
		{
			var nd = GetNodeDefinition(node.Name);
			if (nd.Lambda != null)
				return nd.Lambda(node, nd);
			else
				return GetSimpleTypeValue(nd, node);
		}

		static Object GetSimpleTypeValue(NodeDefinition nd, XamlNode node)
		{
			if (nd.NodeType == typeof(String))
				return node.TextContent;
			else if (nd.NodeType == typeof(Int32))
				return Int32.Parse(node.TextContent, CultureInfo.InvariantCulture);
			else
				throw new XamlReadException($"Unsupported simple type '{nd.NodeType.Name}'");

		}
	}
}
