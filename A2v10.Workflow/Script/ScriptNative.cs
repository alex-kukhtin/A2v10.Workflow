using Jint;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Workflow
{
	public class JsConsole
	{
		public void log(Object msg)
		{
			Console.WriteLine(msg);
		}
	}

	public static class JsNativeExtensions
	{
		public static void AddNativeObjects(this Engine engine)
		{
			engine.SetValue("console", new JsConsole());
		}
	}
}
