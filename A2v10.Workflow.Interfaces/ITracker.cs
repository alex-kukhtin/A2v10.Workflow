﻿using System;

namespace A2v10.Workflow.Interfaces
{
	public interface ITrackRecord
	{
		public DateTime EventTime { get; }
	}

	public interface ITracker
	{
		void Track(ITrackRecord record);
	}
}
