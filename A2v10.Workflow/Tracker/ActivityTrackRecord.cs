using A2v10.Workflow.Interfaces;
using System;

namespace A2v10.Workflow.Tracker
{
	public enum ActivityTrackAction
	{
		Schedule,
		Execute
	}

	public class ActivityTrackRecord : TrackRecord
	{
		private readonly String _message;

		public ActivityTrackRecord(ActivityTrackAction action, IActivity activity, IToken token)
			: base()
		{
			String strToken = token != null ? $", token:'{token}'" : null;
			_message = $"{action} activity: {activity.GetType().Name} {{id: '{activity.Id}'{strToken}}}";
		}

		public override string ToString()
		{
			return _message;
		}
	}
}
