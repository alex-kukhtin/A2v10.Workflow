
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using Jint;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;
	using ResumeAction = Func<IExecutionContext, String, Object, ValueTask>;

	public class QueueItem
	{
		public QueueItem(Func<IExecutionContext, ExecutingAction, ValueTask> action, IActivity activity, ExecutingAction onComplete)
		{
			Action = action;
			Activity = activity;
			OnComplete = onComplete;
		}

		public Func<IExecutionContext, ExecutingAction, ValueTask> Action { get; }
		public IActivity Activity { get; }
		public ExecutingAction OnComplete { get; }
	}

	public class ExecutionContext : IExecutionContext
	{
		private readonly Queue<QueueItem> _commandQueue = new Queue<QueueItem>();
		private readonly ScriptEngine _script;
		private readonly Dictionary<String, IActivity> _activities = new Dictionary<String, IActivity>();
		private readonly Dictionary<String, ResumeAction> _bookmarks = new Dictionary<String, ResumeAction>();
		private readonly IActivity _root;

		public ExecutionContext(IActivity root, Object args = null)
		{
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

			_script = new ScriptEngine(null, _root, sb.Script, args);
		}

		public ExpandoObject GetResult()
		{
			return _script.GetResult();
		}

		#region IExecutionContext
		public void Schedule(IActivity activity, ExecutingAction onComplete)
		{
			_commandQueue.Enqueue(new QueueItem(activity.ExecuteAsync, activity, onComplete));
		}

		public void SetBookmark(String bookmark, ResumeAction onComplete)
		{
			_bookmarks.Add(bookmark, onComplete);
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
				await queueItem.Action.Invoke(this, queueItem.OnComplete);
			}
		}

		public ValueTask ResumeAsync(String bookmark, Object result)
		{
			if (_bookmarks.TryGetValue(bookmark, out ResumeAction action))
			{
				return action.Invoke(this, bookmark, result);
			}
			return new ValueTask();
		}
	}
}
