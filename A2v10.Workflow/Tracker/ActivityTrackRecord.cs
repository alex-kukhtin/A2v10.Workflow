using A2v10.Workflow.Interfaces;
using System;
using System.Dynamic;

namespace A2v10.Workflow.Tracker
{
	public enum ActivityTrackAction
	{
		// see: db
		Schedule = 0,
		Execute  = 1,
		Bookmark = 2,
		Resume   = 3
	}

	public class ActivityTrackRecord : TrackRecord
	{
		private readonly ActivityTrackAction _action;
		private readonly String _id;

		public ActivityTrackRecord(ActivityTrackAction action, IActivity activity, IToken token)
			: base()
		{
			_action = action;
			_id = activity.Id;
			String strToken = token != null ? $", token:'{token}'" : null;
			Message = $"Activity:{action}: {activity.GetType().Name} {{id: '{activity.Id}'{strToken}}}";
		}

		public ActivityTrackRecord(ActivityTrackAction action, String msg)
		{
			_action = action;
			Message = $"Activity:{action}: {msg}";
		}

		public override ExpandoObject ToExpandoObject(int no)
		{
			var eo = CreateExpando(no);
			eo.Set("Kind", (Int32) TrackRecordKind.Activity);
			eo.Set("Action", (Int32) _action);
			eo.Set("Activity", _id);
			return eo;
		}
	}
}
