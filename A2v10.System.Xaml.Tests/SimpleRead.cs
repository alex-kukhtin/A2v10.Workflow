using A2v10.System.Xaml.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace A2v10.System.Xaml.Tests
{
	[TestClass]
	[TestCategory("Xaml.Read.Simple")]
	public class SimpleRead
	{
		[TestMethod]
		public void SimpleProperty()
		{
			string xaml = @"
<Sequence xmlns=""clr-namespace:A2v10.System.Xaml.Tests.Mock;assembly=A2v10.System.Xaml.Tests"" 
	Ref=""Ref0"">
</Sequence>
";
			var obj = XamlServices.Parse(xaml);

			Assert.AreEqual(typeof(Sequence), obj.GetType());
			var sq = obj as Sequence;
			Assert.AreEqual("Ref0", sq.Ref);
		}

		[TestMethod]
		public void SimpleWithChildren()
		{
			string xaml = @"
<Sequence xmlns=""clr-namespace:A2v10.System.Xaml.Tests.Mock;assembly=A2v10.System.Xaml.Tests"" xmlns:sys=""clr-namespace:System;assembly=mscorlib"" 
	Ref=""Ref0"">
<Sequence.Activities>
	<Code Ref=""Ref1"" Script=""X = X + 1""/>
	<Code Ref=""Ref2"" Script=""X = X + 2""/>
	<Code Ref=""Ref3"" Script=""X = X + 3""/>
</Sequence.Activities>
</Sequence>
";
			var obj = XamlServices.Parse(xaml);

			Assert.AreEqual(typeof(Sequence), obj.GetType());
			var sq = obj as Sequence;
			Assert.AreEqual(3, sq.Activities.Count);
			var code0 = sq.Activities[0] as Code;
			Assert.AreEqual("Ref1", code0.Ref);
			Assert.AreEqual("X = X + 1", code0.Script);
			var code2 = sq.Activities[2] as Code;
			Assert.AreEqual("Ref3", code2.Ref);
			Assert.AreEqual("X = X + 3", code2.Script);
		}
	}
}