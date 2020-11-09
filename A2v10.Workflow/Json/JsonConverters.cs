
using System;
using Newtonsoft.Json;

namespace A2v10.Workflow
{
	public class DoubleConverter : JsonConverter<Double>
	{
		public override void WriteJson(JsonWriter writer, Double value, JsonSerializer serializer)
		{
			if (Math.Truncate(value) == value)
				serializer.Serialize(writer, Convert.ToInt64(value));
			else
				serializer.Serialize(writer, value);
		}

		public override Double ReadJson(JsonReader reader, Type objectType, Double existingValue, Boolean hasExistingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<Double>(reader);
		}
	}
}
