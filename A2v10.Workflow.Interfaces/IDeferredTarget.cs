
using System;
using System.Collections.Generic;

namespace A2v10.Workflow.Interfaces
{
	public interface IDeferredTarget
	{
		List<DeferredElement> Deferred { get; }
		String Refer { get; set; }

		void AddDeffered(DeferredElement elem);
	}
}
