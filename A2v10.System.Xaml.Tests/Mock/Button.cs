using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml.Tests.Mock
{
	public class Button : ISupportBinding, ISupportInitialize
	{
		public String Content { get; set; }
		public Object Command { get; set; }

		readonly BindImpl _bindImpl = new BindImpl();

		public BindImpl BindImpl => _bindImpl;

		public Bind GetBinding(string name)
		{
			return _bindImpl?.GetBinding(name);
		}

		public BindCmd GetBindingCommand(string name)
		{
			return _bindImpl?.GetBindingCommand(name);
		}

		public void BeginInit()
		{
		}

		public void EndInit()
		{
			Console.WriteLine("Button.EndInit");
		}
	}
}
