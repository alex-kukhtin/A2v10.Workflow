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
		public PropertyInfo PropertyInfo { get; init; }
		public Type Type { get; init; }
		public Func<Object> Constructor { get; init; }
		public Action<Object, Object> AddMethod { get; init; }
		public Action<Object, String, Object> AddDictionaryMethod { get; init; }
		public Func<String, Object> EnumConvert { get; init; }
		public Func<String, Object> ScalarConvert { get; init; }
	}


	public record NamespaceDefinition
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

	public class NodeBuilder
	{
		private static readonly MethodInfo _getNodePropertyValue =
			typeof(XamlNode).GetMethod("GetPropertyValue", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _initCreatedElement =
			typeof(XamlNode).GetMethod("InitCreatedElement", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _enumParse =
			typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(String) });
		private static readonly MethodInfo _convertChangeType =
			typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(Object), typeof(Type), typeof(CultureInfo) });

		// STATIC????? diff namespaces???
		private readonly ConcurrentDictionary<ClassNamePair, NodeDefinition> _typeCache = new ConcurrentDictionary<ClassNamePair, NodeDefinition>();

		private readonly Dictionary<String, NamespaceDefinition> _namespaces = new Dictionary<String, NamespaceDefinition>();
		private readonly XamlServicesOptions _options;

		public NodeBuilder(XamlServicesOptions options)
		{
			_options = options;
		}

		NamespaceDef IsCustomNamespace(String value)
		{
			if (_options == null)
				return null;
			value = value.ToLowerInvariant();
			return _options.Namespaces.FirstOrDefault(x => x.Name == value);
		}

		public Boolean EnableMarkupExtensions => _options == null || !_options.DisableMarkupExtensions;

		private static readonly Regex _namespaceRegEx = new Regex(@"^\s*clr-namespace\s*:\s*([\w\.]+)\s*;\s*assembly\s*=\s*([\w\.]+)\s*$", RegexOptions.Compiled);

		public void AddNamespace(String prefix, String value)
		{
			if (value == "http://schemas.microsoft.com/winfx/2006/xaml")
			{
				// xaml namespace for x:Key, etc
				return;
			}
			var nsddef = IsCustomNamespace(value);
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

		public MarkupExtension ParseExtension(String value)
		{
			var node = ExtensionParser.Parse(this, value);
			return BuildNode(node) as MarkupExtension;
		}

		private static PropDefinition BuildPropertyDefinition(PropertyInfo propInfo)
		{
			Type propType = propInfo.PropertyType;
			Func<Object> ctor = null;
			Action<Object, Object> addMethod = null;
			Action<Object, String, Object> addDictionaryMethod = null;
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
				if (propCtor != null)
					ctor = Expression.Lambda<Func<Object>>(Expression.New(propCtor)).Compile();
			}
			var mtdAdd = propType.GetMethod("Add");
			if (mtdAdd != null)
			{
				var args = mtdAdd.GetParameters();
				if (args.Length == 1)
				{
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
				else if (args.Length == 2)
				{
					// dictionary
					var argType = args[1].ParameterType;
					var argPrm = Expression.Parameter(typeof(Object));
					var inst = Expression.Parameter(typeof(Object));
					var argKey = Expression.Parameter(typeof(String));

					addDictionaryMethod = Expression.Lambda<Action<Object, String, Object>>(
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
			}

			return new PropDefinition()
			{
				PropertyInfo = propInfo,
				Type = propType,
				Constructor = ctor,
				AddMethod = addMethod,
				AddDictionaryMethod = addDictionaryMethod,
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
				propDefs.Add(prop.Name, BuildPropertyDefinition(prop));

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
				var init = Expression.Call(param, _initCreatedElement, expr);

				// lambda
				lambda = Expression.Lambda<Func<XamlNode, NodeDefinition, Object>>(init/*expr*/, param, nodeDef).Compile();
			}

			return new NodeDefinition()
			{
				ClassName = namePair.ClassName,
				Lambda = lambda,
				Properties = propDefs,
				ContentProperty = nodeType.GetCustomAttribute<ContentPropertyAttribute>()?.Name,
				DefaultProperty = nodeType.GetCustomAttribute<DefaultPropertyAttribute>()?.Name,
				BuildNode = this.BuildNode,
				NodeType = nodeType,
				IsCamelCase = namePair.IsCamelCase
			};
		}

		public String QualifyPropertyName(String name)
		{
			if (!name.Contains(':'))
				return name;
			var name2 = name.Split(':');
			if (!_namespaces.TryGetValue(name2[0], out NamespaceDefinition def))
				throw new XamlReadException($"Namespace '{name2[0]}' not found");
			if (def.IsCamelCase)
				return name2[1].ToPascalCase();
			return name2[1];
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
					ClassName = CheckAlias(typeName),
					IsCamelCase = nsd.IsCamelCase
				};
				return _typeCache.GetOrAdd(className, BuildNodeDefinition);
			}
			else
				throw new XamlReadException($"Namespace '{nsKey}' not found");
		}

		String CheckAlias(String name)
		{
			if (_options == null || _options.Aliases == null)
				return name;
			if (_options.Aliases.TryGetValue(name, out String outName))
				return outName;
			return name;
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
