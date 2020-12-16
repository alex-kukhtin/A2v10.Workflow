using System;
using System.Dynamic;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Interfaces.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace A2v10.Workflow.WebApi
{
	internal record Identity : IIdentity
	{
		public Identity(string id, int ver)
		{
			Id = id;
			Version = ver;
		}

		public string Id { get; }
		public int Version { get; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record Request : IRequest
	{
		
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record Response : IResponse
	{
		[JsonProperty(PropertyName = "success")]
		public bool? Status { get; } = null;

		public Response(bool statusFlag, IResponse _)
		{
			if (statusFlag)
				Status = true;
		}

		public Response(bool statusFlag, ApiException _)
		{
			if (statusFlag)
				Status = false;
		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record ErrorResponse : Response
	{
		public ErrorResponse(ApiException e, bool statusFlag = true) : base(statusFlag, e)
		{

		}
	}
}