using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml.Tests.Mock
{
	public enum Icon
	{
		Undefined,
		File,
		Folder
	}

	public class Button :  UIElementBase, ISupportInitialize
	{
		public String Content { get; set; }
		public Object Command { get; set; }

		public Icon Icon { get; set; }


		public void BeginInit()
		{
		}

		public void EndInit()
		{
			Console.WriteLine("Button.EndInit");
		}
	}
}
