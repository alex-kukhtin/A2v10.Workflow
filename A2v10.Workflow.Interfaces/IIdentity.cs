﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IIdentity
	{
		String Id { get; }
		Int32 Version { get; }
	}
}