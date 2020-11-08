using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow.Interfaces
{
	public interface IStorable
	{
		void Store(IActivityStorage storage);
		void Restore(IActivityStorage storage);
	}
}
