using A2v10.System.Xaml.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.System.Xaml.Tests
{
	[TestClass]
	[TestCategory("Xaml.Extensions.Simple")]
	public class ReadExtensions
	{
		[TestMethod]
		public void SimpleExtension()
		{
			string xaml = @"
<Button xmlns=""clr-namespace:A2v10.System.Xaml.Tests.Mock;assembly=A2v10.System.Xaml.Tests"" 
	Content=""Text"" Command=""{BindCmd Execute, Argument={Bind Text}}"">
</Button>
";
			var obj = XamlServices.Parse(xaml, null);

			Assert.AreEqual(typeof(Button), obj.GetType());
			var btn= obj as Button;
			Assert.AreEqual("Text", btn.Content);

			var cmd = btn.GetBindingCommand("Command");
			Assert.AreEqual(typeof(BindCmd), cmd.GetType());
			//Assert.AreEqual("Execute", cmd.Name);
		}
	}
}
