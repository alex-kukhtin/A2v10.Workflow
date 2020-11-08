using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Tests.Runtime
{
	[TestClass]
	[TestCategory("Runtime.Store")]
	public class StoreActivity : IActivity
	{
		// fake implementation for target check
		public String Ref => "Root";
		public ValueTask Execute(IExecutionContext context, Func<IExecutionContext, IActivity, ValueTask> onComplete)
		{
			throw new NotImplementedException();
		}

		public ValueTask TraverseAsync(Func<IActivity, ValueTask> onAction)
		{
			return new ValueTask();
		}

		public void Traverse(Action<IActivity> onAction)
		{
		}

		public ValueTask Cancel(IExecutionContext context)
		{
			return new ValueTask();
		}



		[TestMethod]
		public void StoreSequence()
		{
			var s = new Sequence()
			{
				Ref = "Ref0",
				Activities = new List<IActivity>()
				{
					new Code() {Ref="Ref1", Script="print ref1"},
					new Code() {Ref="Ref1", Script="print ref2"},
				}
			};

			var context = new ExecutionContext();
			s.Execute(context, OnElemComplete);

			var mockStorage = new ActivityStorageMock();
			var storable = s as IStorable;
			storable.Store(mockStorage);
		}

		[StoreName("OnComplete")]
		private ValueTask OnElemComplete(IExecutionContext context, IActivity activity)
		{
			return new ValueTask();
		}
	}
}
