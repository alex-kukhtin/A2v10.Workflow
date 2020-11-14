
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using A2v10.Workflow.Tracker;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;
	using ResumeAction = Func<IExecutionContext, String, Object, ValueTask>;

	public record QueueItem
	(
		Func<IExecutionContext, ExecutingAction, ValueTask> Action, 
		IActivity Activity, 
		ExecutingAction OnComplete
	);

	public partial class ExecutionContext : IExecutionContext
	{
		private readonly Queue<QueueItem> _commandQueue = new Queue<QueueItem>();
		private readonly Dictionary<String, IActivity> _activities = new Dictionary<String, IActivity>();
		private readonly Dictionary<String, ResumeAction> _bookmarks = new Dictionary<String, ResumeAction>();
		private readonly IActivity _root;

		private readonly ScriptEngine _script;
		private readonly ITracker _tracker;

		public ExecutionContext(ITracker tracker, IActivity root, Object args = null)
		{
			_tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
			_root = root;
			// store all activites
			var toMapArg = new TraverseArg()
			{
				Action = (activity) => _activities.Add(activity.Ref, activity)
			};
			_root.Traverse(toMapArg);

			var sb = new ScriptBuilder();
			var sbTraverseArg = new TraverseArg()
			{
				Start = (activity) => sb.Start(activity),
				Action = (activity) => sb.Build(activity),
				End = (activity) => sb.End(activity)
			};
			_root.Traverse(sbTraverseArg);
			sb.EndScript();

			_script = new ScriptEngine(_root, sb.Script, args);
		}

		public ExpandoObject GetResult()
		{
			return _script.GetResult();
		}


		#region IExecutionContext
		public void Schedule(IActivity activity, ExecutingAction onComplete)
		{
			if (activity == null)
				return;
			_tracker.Track(new ActivityTrackRecord(ActivityTrackAction.Schedule, activity));
			_commandQueue.Enqueue(new QueueItem(activity.ExecuteAsync, activity, onComplete));
		}

		public void SetBookmark(String bookmark, ResumeAction onComplete)
		{
			_bookmarks.Add(bookmark, onComplete);
		}

		public void RemoveBookmark(String bookmark)
		{
			if (_bookmarks.ContainsKey(bookmark))
				_bookmarks.Remove(bookmark);
		}

		public T Evaluate<T>(String refer, String name)
		{
			return _script.Evaluate<T>(refer, name);
		}

		public void Execute(String refer, String name)
		{
			_script.Execute(refer, name);
		}

		#endregion

		public async ValueTask RunAsync()
		{
			while (_commandQueue.Count > 0)
			{
				var queueItem = _commandQueue.Dequeue();
				_tracker.Track(new ActivityTrackRecord(ActivityTrackAction.Execute, queueItem.Activity));
				await queueItem.Action(this, queueItem.OnComplete);
			}
		}

		public ValueTask ResumeAsync(String bookmark, Object result)
		{
			if (_bookmarks.TryGetValue(bookmark, out ResumeAction action))
			{
				return action(this, bookmark, result);
			}
			return new ValueTask();
		}
	}
}
