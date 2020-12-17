using A2v10.Workflow.Interfaces;
using System;

namespace A2v10.Workflow.Tracker
{
	public enum ScriptTrackAction
	{
		Evaluate,
		Execute
	}

	public class ScriptTrackRecord : TrackRecord
	{
		private readonly String _message;

		public ScriptTrackRecord(ScriptTrackAction action, String refer, String name)
			: base()
		{
			_message = $"Script:{action}: {{id: {refer}, name: '{name}'}}";
		}

		public override string ToString()
		{
			return _message;
		}
	}
}
