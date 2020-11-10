using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{

	public class TraverseArg
	{
		public Action<IActivity> Start;
		public Action<IActivity> Action;
		public Action<IActivity> End;
	}

	public static class ActivityExtensions
	{
		public static void Traverse(this IActivity activity, TraverseArg traverse)
		{
			traverse.Start?.Invoke(activity);
			traverse.Action?.Invoke(activity);
			foreach (var ch in activity.EnumChildren())
				ch.Traverse(traverse);
			traverse.End?.Invoke(activity);
		}
	}
}
