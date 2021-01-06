
using System;
using Jint;
using Jint.Runtime.Interop;

namespace A2v10.Workflow
{
	public class JsConsole
	{
#pragma warning disable IDE1006 // Naming Styles
		public static void log(Object msg)
#pragma warning restore IDE1006 // Naming Styles
		{
			Console.WriteLine(msg);
		}
	}

	public static class JsNativeExtensions
	{
		public static void AddNativeObjects(this Engine engine)
		{
			engine.SetValue("console", new JsConsole());
			engine.SetValue("Database", TypeReference.CreateTypeReference(engine, typeof(ScriptDatabase)));
		}
	}
}
