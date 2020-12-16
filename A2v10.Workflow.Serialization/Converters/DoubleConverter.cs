﻿
using System;

using Newtonsoft.Json;

namespace A2v10.Workflow.Serialization
{
	public class DoubleConverter : JsonConverter<Double>
	{
		public override void WriteJson(JsonWriter writer, Double value, JsonSerializer serializer)
		{
			if (Double.IsNaN(value))
				writer.WriteValue("NaN");
			else if (Double.IsInfinity(value))
				writer.WriteValue("Infinity");
			else if (Math.Truncate(value) == value)
				writer.WriteValue(Convert.ToInt64(value));
			else
				writer.WriteValue(value);
		}

		public override Double ReadJson(JsonReader reader, Type objectType, Double existingValue, Boolean hasExistingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<Double>(reader);
		}
	}
}
