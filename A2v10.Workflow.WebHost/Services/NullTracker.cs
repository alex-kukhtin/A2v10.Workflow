using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.WebHost
{
	public class NullTracker : ITracker
	{
		public void Track(ITrackRecord record)
		{
		}

		public List<ITrackRecord> Records => null;

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
