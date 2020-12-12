using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	public class Workflow : IWorkflow
	{
		public IIdentity Identity { get; init; }
		public IActivity Root { get; init; }
	}
}
