using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	public class InstanceData : IInstanceData
	{
		public ExpandoObject ExternalVariables { get; init; }

		public List<Object> ExternalBookmarks { get; init; }

		public List<Object> TrackRecords { get; init; }
	}
}
