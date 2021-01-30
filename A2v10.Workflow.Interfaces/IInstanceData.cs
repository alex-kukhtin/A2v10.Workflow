
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstanceData
	{
		ExpandoObject ExternalVariables { get; }
		List<Object> ExternalBookmarks { get; }
		List<Object> TrackRecords { get; }
	}
}
