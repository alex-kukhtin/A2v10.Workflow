﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class Error : BaseElement
	{
		public String Name { get; init; }
		public String ErrorCode { get; init; }
	}
}