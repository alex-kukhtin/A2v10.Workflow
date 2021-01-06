using A2v10.Workflow.Interfaces;
using System;

namespace A2v10.Workflow.Tracker
{
	public enum ActivityTrackAction
	{
		Schedule,
		Execute,
		Bookmark,
		Resume
	}

	public class ActivityTrackRecord : TrackRecord
	{
		private readonly String _message;

		public ActivityTrackRecord(ActivityTrackAction action, IActivity activity, IToken token)
			: base()
		{
			String strToken = token != null ? $", token:'{token}'" : null;
			_message = $"Activity:{action}: {activity.GetType().Name} {{id: '{activity.Id}'{strToken}}}";
		}

		public ActivityTrackRecord(ActivityTrackAction action, String msg)
		{
			_message = $"Activity:{action}: {msg}";
		}

		public override string ToString()
		{
			return _message;
		}
	}
}
