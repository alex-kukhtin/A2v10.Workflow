using System;
using System.Collections.Generic;
using System.Dynamic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Serialization
{
	public record ActivityWrapper
	{
		public IActivity Root { get; init; }
	}

	public class Serializer : ISerializer
	{
		private readonly JsonSerializerSettings _actititySettings = new JsonSerializerSettings()
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
			Converters = new List<JsonConverter>() { new StringEnumConverter(), new DoubleConverter() }
		};

		private readonly JsonConverter[] _jsonConverters = new JsonConverter[]
		{
			new DoubleConverter()
		};


		public ExpandoObject Deserialize(String text)
		{
			return JsonConvert.DeserializeObject<ExpandoObject>(text, _jsonConverters);
		}

		public String Serialize(ExpandoObject obj)
		{
			return JsonConvert.SerializeObject(obj, _jsonConverters);
		}

		public IActivity DeserializeActitity(String text, String format)
		{
			return format switch
			{
				"json" => JsonConvert.DeserializeObject<ActivityWrapper>(text, _actititySettings).Root,
				"xaml" => DeserializeXaml(text),
				_ => throw new NotImplementedException($"Deserialize for format '{format}' is not supported"),
			};
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
			return null;
		}
	}
}
