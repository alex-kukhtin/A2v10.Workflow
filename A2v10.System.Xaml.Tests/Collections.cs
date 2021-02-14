using A2v10.System.Xaml.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.System.Xaml.Tests
{
	[TestClass]
	[TestCategory("Xaml.Collections")]
	public class Collections
	{
		[TestMethod]
		public void SimpleCollection()
		{
			string xaml = @"
<Page xmlns=""clr-namespace:A2v10.System.Xaml.Tests.Mock;assembly=A2v10.System.Xaml.Tests"" Title=""PageTitle"">
	<Block If=""False"" Show=""True"">
		<Button />
		<Span>I am the span text</Span>
	</Block>
	<Block>
		I am the block text
	</Block>
</Page>
";
			var obj = XamlServices.Parse(xaml, null);

			Assert.AreEqual(typeof(Page), obj.GetType());
			var p = obj as Page;

			var c = p.Children;
			Assert.AreEqual(2, c.Count);

			var c1 = p.Children[0];
			Assert.AreEqual(typeof(Block), c1.GetType());

			var b1 = c1 as Block;
			Assert.AreEqual(true, b1.Show);
			Assert.AreEqual(false, b1.If);
		}
	}
}
