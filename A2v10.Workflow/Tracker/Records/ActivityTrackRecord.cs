
using System;
using System.Dynamic;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tracker
{
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
			Message = $"{action}: {activity.GetType().Name} {{id: '{activity.Id}'{strToken}}}";
		}

		public ActivityTrackRecord(ActivityTrackAction action, String msg)
		{
			_action = action;
			Message = $"{action}: {msg}";
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
