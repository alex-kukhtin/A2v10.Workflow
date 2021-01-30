using System;
using System.Dynamic;

namespace TestSite.Models
{
	public class InstanceOpenModel
	{
		public String Id { get; set; }

		public String State { get; set; }

		public dynamic Instance { get; set; }
	}
}
