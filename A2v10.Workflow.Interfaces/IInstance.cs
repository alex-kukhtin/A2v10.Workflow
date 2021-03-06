﻿
using System;
using System.Dynamic;

namespace A2v10.Workflow.Interfaces
{

	public enum WorkflowExecutionStatus
	{
		Init,
		Idle,
		Complete,
		Faulted
	}

	public interface IInstance
	{
		IWorkflow Workflow { get; }

		Guid Id { get; }
		Guid? Parent { get; }

		WorkflowExecutionStatus ExecutionStatus { get; set; }
		Guid? Lock { get; }

		ExpandoObject Result { get; set; }
		ExpandoObject State { get; set; }

		IInstanceData InstanceData { get; set; }
	}
}
