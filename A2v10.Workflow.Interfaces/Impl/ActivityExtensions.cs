using System;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public static class ActivityExtensions
	{
		public static async ValueTask TraverseAsync(this IActivity activity, Func<IActivity, ValueTask> onAction)
		{
			await onAction(activity);
			foreach (var ch in activity.EnumChildren())
				await ch.TraverseAsync(onAction);
		}
	}
}
