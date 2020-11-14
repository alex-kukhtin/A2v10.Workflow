using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace A2v10.System.Xaml.Tests
{
	[TestClass]
	[TestCategory("Xaml.Simple.Read")]
	public class SimpleRead
	{
		[TestMethod]
		public void SimpleString()
		{
			string xaml = @"
<Sequence xmlns=""clr-namespace:A2v10.Workflow;assembly=A2v10.Workflow"" Ref=""Ref0"">
<Sequence.Activities>
	<Code Ref=""Ref1"" Script=""X = X + 1""/>
	<Code Ref=""Ref2"" Script=""X = X + 1""/>
	<Code Ref=""Ref3"" Script=""X = X + 1""/>
</Sequence.Activities>
</Sequence>
";
			var obj = XamlServices.Parse(xaml);

			Assert.AreEqual(typeof(A2v10.Workflow.Sequence), obj.GetType());

			int z = 55;
		}
	}
}
