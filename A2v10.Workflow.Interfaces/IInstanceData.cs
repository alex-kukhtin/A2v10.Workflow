using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstanceData
	{
		ExpandoObject ExternalVariables { get; }
		ExpandoObject ExternalBookmarks { get; }
		List<Object> TrackRecords { get; }
	}
}
