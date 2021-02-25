﻿
using System;
using System.Dynamic;
using System.Text.Json;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tracker
{
	public enum ScriptTrackAction
	{
		// see: db
		Evaluate = 0,
		EvaluateResult = 1,
		Execute = 2,
		ExecuteResult = 3
	}

	public class ScriptTrackRecord : TrackRecord
	{
		private readonly ScriptTrackAction _action;

		public ScriptTrackRecord(ScriptTrackAction action, String refer, String name, Object result = null)
			: base()
		{
			_action = action;
			String strResult = result != null ? $", result:{JsonSerializer.Serialize(result)}" : String.Empty;
			Message = $"Script:{action}: {{id: {refer}, name: '{name}'{strResult}}}";
		}

		public override ExpandoObject ToExpandoObject(int no)
		{
			var eo = CreateExpando(no);
			eo.Set("Kind", (Int32) TrackRecordKind.Script);
			eo.Set("Action", (Int32) _action);
			return eo;
		}
	}
}
