﻿
using System;

using A2v10.System.Xaml;

namespace A2v10.Workflow.Bpmn
{
	[ContentProperty("Expression")]
	public abstract class TimeBase : BaseElement
	{
		public String Type { get; init; }
		public String Expression { get; init; }

		public abstract Boolean CanRepeat { get; }
		public abstract DateTime NextTriggerTime { get; }
	}
}
