
using System;

namespace A2v10.Workflow.Tracker
{
	public enum ScriptTrackAction
	{
		Evaluate,
		EvaluateResult,
		Execute
	}

	public class ScriptTrackRecord : TrackRecord
	{
		private readonly String _message;

		public ScriptTrackRecord(ScriptTrackAction action, String refer, String name, Object result = null)
			: base()
		{
			String strResult = result != null ? $", result:{result}" : String.Empty;
			_message = $"Script:{action}: {{id: {refer}, name: '{name}'{strResult}}}";
		}

		public override String ToString()
		{
			return _message;
		}
	}
}
