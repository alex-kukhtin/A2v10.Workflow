using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces.Api
{
	internal record Response : IResponse
	{
	}

	internal record StartProcessResponse : Response, IStartProcessResponse, ICreateProcessResponse
	{
		public Guid InstanceId { get; init; }
	}

	internal record ResumeProcessResponse : Response, IResumeProcessResponse, IRunProcessResponse
	{
	}
}