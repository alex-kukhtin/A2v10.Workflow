﻿using System;
using System.Dynamic;

using Microsoft.Extensions.DependencyInjection;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow
{
	public class ScriptNativeDeferred : IInjectable
	{
		private IDeferredTarget _deferredTarget;

		public void Inject(IServiceProvider serviceProvider)
		{
			_deferredTarget = serviceProvider.GetService<IDeferredTarget>() ?? throw new NullReferenceException("IDeferredTarget");
		}

#pragma warning disable IDE1006 // Naming Styles
		public void executeSql(String procedure, ExpandoObject prms = null)
#pragma warning restore IDE1006 // Naming Styles
		{
			_deferredTarget.AddDeffered(new DeferredElement(DeferredElementType.Sql, procedure, prms, _deferredTarget.Refer));
		}
	}
}
