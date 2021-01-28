using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces.Api
{
	public interface IWorkflowApi
	{
		ValueTask<ICreateProcessResponse> CreateAsync(ICreateProcessRequest prm);
		ValueTask<IRunProcessResponse> RunAsync(IRunProcessRequest prm);
		ValueTask<IStartProcessResponse> StartAsync(IStartProcessRequest prm);
		ValueTask<IResumeProcessResponse> ResumeAsync(IResumeProcessRequest prm);
	}

	public interface IRequest
	{
		
	}

	public interface IResponse
	{

	}

	public interface ICreateProcessRequest : IRequest
	{
		IIdentity Identity { get; }
	}

	public interface ICreateProcessResponse : IResponse
	{
		Guid InstanceId { get; }
	}

	public interface IRunProcessRequest : IRequest
	{
		Guid InstanceId { get; }
		Object Parameters { get; }
	}

	public interface IRunProcessResponse : IResponse
	{
		
	}

	public interface IStartProcessRequest : IRequest
	{
		IIdentity Identity { get; }
		Object Parameters { get; }
	}

	public interface IStartProcessResponse : IResponse
	{
		Guid InstanceId { get; }
	}

	public interface IResumeProcessRequest : IRequest
	{
		Guid InstanceId { get; }
		String Bookmark { get; }
		Object Result { get; }
	}

	public interface IResumeProcessResponse : IResponse
	{

	}
}
