﻿using System;
using System.Collections.Generic;
using System.Dynamic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using A2v10.Workflow.Interfaces;
using A2v10.System.Xaml;

namespace A2v10.Workflow.Serialization
{
	public record ActivityWrapper
	{
		public IActivity Root { get; init; }
	}

	public class Serializer : ISerializer
	{
		private readonly IXamlReaderService _xamlCodeProvider;

		public Serializer(IXamlReaderService xamlCodeProvider)
		{
			_xamlCodeProvider = xamlCodeProvider;
		}

		private static readonly JsonConverter[] _jsonConverters = new JsonConverter[]
		{
			new DoubleConverter(),
			new StringEnumConverter(),
			new ExpandoObjectConverter()
		};

		private static readonly JsonSerializerSettings _actititySettings = new JsonSerializerSettings()
		{
			Formatting = Formatting.None,
			NullValueHandling = NullValueHandling.Ignore,
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy
				{
					OverrideSpecifiedNames = false
				}
			},
			TypeNameHandling = TypeNameHandling.Auto,
			Converters = _jsonConverters
		};

		private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
		{
			Formatting = Formatting.None,
			NullValueHandling = NullValueHandling.Ignore,
			FloatFormatHandling = FloatFormatHandling.Symbol,
			FloatParseHandling = FloatParseHandling.Decimal,
			Converters = _jsonConverters
		};


		public ExpandoObject Deserialize(String text)
		{
			if (text == null)
				return null;
			return JsonConvert.DeserializeObject<ExpandoObject>(text, _jsonSettings);
		}

		public String Serialize(ExpandoObject obj)
		{
			return JsonConvert.SerializeObject(obj, _jsonSettings);
		}

		public IActivity DeserializeActitity(String text, String format)
		{
			var activitiy = format switch
			{
				"json" => JsonConvert.DeserializeObject<ActivityWrapper>(text, _actititySettings).Root,
				"xaml" => DeserializeXaml(text),
				"text/xml" => DeserializeXaml(text),
				_ => throw new NotImplementedException($"Deserialize for format '{format}' is not supported"),
			};
			activitiy.OnEndInit();
			return activitiy;
		}


		public String SerializeActitity(IActivity activity, String format)
		{
			return format switch
			{
				"json" => JsonConvert.SerializeObject(new ActivityWrapper() { Root = activity }, _actititySettings),
				_ => throw new NotImplementedException($"Deserialize for format '{format}' is not supported"),
			};
		}

		IActivity DeserializeXaml(String text)
		{
			var obj = _xamlCodeProvider.ParseXml(text);
			return obj switch
			{
				IActivityWrapper activityWrapper => activityWrapper.Root(),
				IActivity activity => activity,
				_ => null,
			};
		}
	}
}
