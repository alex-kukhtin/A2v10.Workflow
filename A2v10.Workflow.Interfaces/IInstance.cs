
using System;
using System.Dynamic;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstance
	{
		Guid Id { get; }
		Guid Parent { get; }
		IActivity Root { get; }

		ExpandoObject Result { get; set; }
		ExpandoObject State { get; set; }
	}
}
