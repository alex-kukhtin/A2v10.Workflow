using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSite
{
	public class NullTracker : ITracker
	{
		public void Track(ITrackRecord record)
		{
		}
	}
}
