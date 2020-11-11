﻿using System;
using System.Collections.Generic;
using System.Text;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public abstract class StateBase : Activity
	{
		public virtual Boolean IsStart => false;
		public virtual Boolean IsFinal => false;

		public String Next { get; set; }

		internal String NextState { get; set; }
	}
}
