
using System;
using System.Dynamic;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class Instance : IInstance
	{
		public Guid Id { get; set; }
		public Guid Parent { get; set; }
		public IActivity Root { get; set; }
	
		public ExpandoObject Result { get; set; }
		public ExpandoObject State { get; set; }
	}
}
