﻿using System;
using System.Dynamic;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Interfaces.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace A2v10.Workflow.WebApi
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record CreateProcessRequest : Request, ICreateProcessRequest
	{
		[JsonProperty(PropertyName = "workflow")]
		public string Workflow { get; set; }
		[JsonProperty(PropertyName = "version")]
		public int Version { get; set; }

		IIdentity ICreateProcessRequest.Identity => new Identity(Workflow, Version);
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record CreateProcessResponse : Response, ICreateProcessResponse
	{
		public CreateProcessResponse(ICreateProcessResponse src, bool statusFlag = true) : base(statusFlag, src)
		{
			InstanceId = src.InstanceId;
		}

		[JsonProperty(PropertyName = "instanceId")]
		public Guid InstanceId { get; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record RunProcessRequest : Request, IRunProcessRequest
	{
		[JsonProperty(PropertyName = "instanceId")]
		public Guid InstanceId { get; set; }
		[JsonProperty(PropertyName = "parameters")]
		[JsonConverter(typeof(ExpandoObjectConverter))]
		public ExpandoObject Parameters { get; set; }

		object IRunProcessRequest.Parameters => Parameters;
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record RunProcessResponse : Response
	{
		public RunProcessResponse(IRunProcessResponse src, bool statusFlag = true) : base(statusFlag, src)
		{

		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record StartProcessRequest : Request, IStartProcessRequest
	{
		[JsonProperty(PropertyName = "workflow")]
		public string Workflow { get; set; }
		[JsonProperty(PropertyName = "version")]
		public int Version { get; set; }
		[JsonProperty(PropertyName = "parameters")]
		[JsonConverter(typeof(ExpandoObjectConverter))]
		public ExpandoObject Parameters { get; set; }

		IIdentity IStartProcessRequest.Identity => new Identity(Workflow, Version);
		object IStartProcessRequest.Parameters => Parameters;
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record StartProcessResponse : Response, IStartProcessResponse
	{
		public StartProcessResponse(IStartProcessResponse src, bool statusFlag = true) : base(statusFlag, src)
		{
			InstanceId = src.InstanceId;
		}

		[JsonProperty(PropertyName = "instanceId")]
		public Guid InstanceId { get; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record ResumeProcessRequest : Request, IResumeProcessRequest
	{
		[JsonProperty(PropertyName = "instanceId")]
		public Guid InstanceId { get; set; }
		[JsonProperty(PropertyName = "bookmark")]
		public string Bookmark { get; set; }
		[JsonProperty(PropertyName = "result")]
		[JsonConverter(typeof(ExpandoObjectConverter))]
		public Object Result { get; set; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public record ResumeProcessResponse : Response
	{
		public ResumeProcessResponse(IResumeProcessResponse src, bool statusFlag = true) : base(statusFlag, src)
		{

		}
	}
}