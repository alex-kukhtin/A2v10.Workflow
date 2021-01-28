﻿using A2v10.Workflow.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests.Serialization
{
	[TestClass]
	[TestCategory("Converters.Test")]
	public class TestConverters
	{
		private readonly JsonSerializerSettings settings = new JsonSerializerSettings
		{
			Converters = new JsonConverter[]
			{
				new DoubleConverter()
			}
		};

		public class Test
		{
			public Double x;
			public Double y;
			public Double n;
		}
		

		[TestMethod]
		public void TestDoubleConverter()
		{
			var s = new { x = 5, y = 7.12, n = Double.NaN };
			var x = JsonConvert.SerializeObject(s, settings);
			Console.WriteLine(x);
			var z = JsonConvert.DeserializeObject<Test>(x);
			Assert.AreEqual("{\"x\":5,\"y\":7.12,\"n\":\"NaN\"}", x);
			Assert.AreEqual(s.x, z.x);
			Assert.AreEqual(s.y, z.y);
			Assert.AreEqual(s.n, z.n);
			Assert.AreEqual(z.n, Double.NaN);
		}
	}
}
