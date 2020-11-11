using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Tracker
{
	public enum ActivityTrackAction
	{
		Schedule,
		Execute
	}

	public class ActivityTrackRecord : TrackRecord
	{
		String _message;
		public ActivityTrackRecord(ActivityTrackAction action, IActivity activity)
			: base()
		{
			_message = $"{action} activity: {activity.GetType().Name}[{activity.Ref}]";
		}

		public override string ToString()
		{
			return _message;
		}
	}
}
