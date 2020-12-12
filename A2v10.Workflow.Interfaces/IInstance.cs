﻿
using System;
using System.Dynamic;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstance
	{
		IWorkflow Workflow { get; }

		Guid Id { get; }
		Guid Parent { get; }

		ExpandoObject Result { get; set; }
		ExpandoObject State { get; set; }
	}
}
