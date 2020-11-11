using System;
using System.Collections.Generic;
using System.Text;
using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class ConsoleTracker : ITracker
	{
		Int32 _no;

		public ConsoleTracker()
		{
			_no = 1;
		}

		public void Track(ITrackRecord record)
		{
			Console.WriteLine($"{_no++}: {record}");
		}
	}
}
