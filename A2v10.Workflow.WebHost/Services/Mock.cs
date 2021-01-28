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

		private class StartResp : IStartProcessResponse, ICreateProcessResponse
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

		public ValueTask<ICreateProcessResponse> CreateAsync(ICreateProcessRequest prm)
		{
			if (prm.Identity.Id == "test")
				return new ValueTask<ICreateProcessResponse>(new StartResp());
			throw new ApiException();
		}

		public ValueTask<IRunProcessResponse> RunAsync(IRunProcessRequest prm)
		{
			throw new ApiException();
		}
	}
}
