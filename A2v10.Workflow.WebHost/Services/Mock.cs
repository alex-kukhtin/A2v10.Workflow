using A2v10.Workflow.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2v10.Workflow.WebHost.Services
{
	public class MockApi : IWorkflowApi
	{
		public ValueTask<IResumeProcessResponse> ResumeAsync(IResumeProcessRequest prm)
		{
			throw new ApiException();
		}

		private class StartResp : IStartProcessResponse
		{
			public StartResp()
			{
				InstanceId = Guid.NewGuid();
			}
			public Guid InstanceId { get; }
		}

		public ValueTask<IStartProcessResponse> StartAsync(IStartProcessRequest prm)
		{
			if (prm.Identity.Id == "test")
				return new ValueTask<IStartProcessResponse>(new StartResp());
			throw new ApiException();
		}
	}
}
