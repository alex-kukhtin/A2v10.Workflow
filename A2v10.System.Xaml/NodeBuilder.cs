
using System;
using System.Linq;
using System.Linq.Expressions;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace A2v10.System.Xaml
{
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

		// STATIC????? diff namespaces???
		private readonly ConcurrentDictionary<ClassNamePair, TypeDescriptor> _descriptorCache = new ConcurrentDictionary<ClassNamePair, TypeDescriptor>();

		private readonly Dictionary<String, NamespaceDefinition> _namespaces = new Dictionary<String, NamespaceDefinition>();
		private readonly XamlServicesOptions _options;
		private readonly XamlServiceProvider _serviceProvider;

		public NodeBuilder(XamlServiceProvider serviceProvider, XamlServicesOptions options)
		{
			_serviceProvider = serviceProvider;
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
				_namespaces.Add(prefix, new NamespaceDefinition()
				{
				});
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

		private static PropertyDescriptor BuildPropertyDefinition(PropertyInfo propInfo)
		{
			Type propType = propInfo.PropertyType;
			Func<Object> ctor = null;
			Action<Object, Object> addMethod = null;
			Func<TypeConverter> typeConverter = null;

			if (propType != typeof(String))
			{
				var propCtor = propType.GetConstructor(Array.Empty<Type>());
				if (propCtor != null)
					ctor = Expression.Lambda<Func<Object>>(Expression.New(propCtor)).Compile();
				var conv = propType.GetCustomAttribute<TypeConverterAttribute>();
				if (conv != null) {
					var convCtor = Type.GetType(conv.ConverterTypeName).GetConstructor(Array.Empty<Type>());
					typeConverter = Expression.Lambda<Func<TypeConverter>>(Expression.New(convCtor)).Compile();
				}
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
			}

			return new PropertyDescriptor()
			{
				PropertyInfo = propInfo,
				//Type = propType,
				Constructor = ctor,
				TypeConverter = typeConverter,
				AddMethod = addMethod,
				//AddDictionaryMethod = addDictionaryMethod,
			};
		}

		TypeDescriptor BuildTypeDescriptor(ClassNamePair namePair)
		{
			if (!_namespaces.TryGetValue(namePair.Prefix, out NamespaceDefinition nsd))
				throw new XamlReadException($"Namespace {namePair.Namespace} not found");
			var typeName = $"{namePair.Namespace}.{namePair.ClassName}";
			var nodeType = nsd.Assembly.GetType(typeName);
			if (nodeType == null)
				throw new XamlReadException($"Class {namePair.Namespace}.{namePair.ClassName} not found");
			Func<Object> constructor = null;
			Func<String, Object> constructorStr = null;
			var ctor0 = nodeType.GetConstructor(Array.Empty<Type>());
			if (ctor0 != null)
				constructor = Expression.Lambda<Func<Object>>(Expression.New(nodeType)).Compile();
			var ctorStr = nodeType.GetConstructor(new Type[] { typeof(String) });
			if (ctorStr != null)
			{
				var prm = Expression.Parameter(typeof(String));
				constructorStr = Expression.Lambda<Func<String, Object>>(
					Expression.New(ctorStr, prm), 
					prm
				).Compile();
			}
			var props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			MethodInfo addCollection1 = null;
			MethodInfo addCollection2 = null;

			var contentProperty = nodeType.GetCustomAttribute<ContentPropertyAttribute>()?.Name;

			if (contentProperty != null)
			{
				var contProp = props.Where(x => x.Name == contentProperty).FirstOrDefault();
				if (contProp == null)
					throw new XamlException($"Property {contentProperty} not found in type {typeName}");
				var mtdAdd = contProp.PropertyType.GetMethod("Add");
				if (mtdAdd != null)
				{
					var mtdAddPraramCount = mtdAdd.GetParameters().Length;
					if (mtdAddPraramCount == 1)
						addCollection1 = mtdAdd;
					else if (mtdAddPraramCount == 2)
						addCollection2 = mtdAdd;
				}
			}

			var propDefs = new Dictionary<String, PropertyDescriptor>();
			foreach (var prop in props)
			{
				propDefs.Add(prop.Name, BuildPropertyDefinition(prop));
			}

			return new TypeDescriptor()
			{
				NodeType = nodeType,
				TypeName = typeName,
				IsCamelCase = namePair.IsCamelCase,
				BuildNode = this.BuildNode,
				Constructor = constructor,
				ConstructorString = constructorStr,
				Properties = propDefs,
				ContentProperty = contentProperty,
				AddCollection1 = addCollection1,
				//AddCollection2 = addCollection2
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

		public TypeDescriptor GetNodeDescriptor(String typeName)
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
				return _descriptorCache.GetOrAdd(className, BuildTypeDescriptor);
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
			var nd = GetNodeDescriptor(node.Name);
			Object obj = null;
			if (node.ConstructorArgument != null)
				obj = nd.ConstructorString(node.ConstructorArgument);
			else if (nd.Constructor != null)
				obj = nd.Constructor();
			else
				return Convert.ChangeType(node.TextContent, nd.NodeType); // Simple string ????

			if (!String.IsNullOrEmpty(node.TextContent))
				nd.SetTextContent(obj, node.TextContent);

			foreach (var (propKey, propValue) in node.Properties)
			{
				nd.SetPropertyValue(obj, propKey, propValue);
			}
			if (node.HasChildren)
			{
				foreach (var ch in node.Children.Value)
				{
					var chObj = BuildNode(ch);
					nd.AddChildren(obj, chObj);
				}
			}
			if (node.Extensions != null)
			{
				var valueTarget = _serviceProvider.ProvideValueTarget;
				valueTarget.TargetObject = obj;
				foreach (var ext in node.Extensions)
				{
					valueTarget.TargetProperty = ext.PropertyInfo;
					ext.Element.ProvideValue(_serviceProvider);
				}
			}

			if (obj is ISupportInitialize init) {
				init.BeginInit();
				init.EndInit();
			}
			return obj;
		}
	}
}
