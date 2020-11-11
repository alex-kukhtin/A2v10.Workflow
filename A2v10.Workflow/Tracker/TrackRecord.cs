using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Tracker
{
	public class TrackRecord : ITrackRecord
	{
		public DateTime EventTime { get; }
		public Int32 RecordNumber { get; set; }

		public TrackRecord()
		{
			EventTime = DateTime.UtcNow;
		}
	}
}
