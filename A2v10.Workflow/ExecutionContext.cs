
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;
using Jint;

namespace A2v10.Workflow
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;

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
		private readonly Engine _engine = new Engine(EngineOptions);

		private static void EngineOptions(Options opts)
		{
			opts.Strict(true);
		}

		#region IExecutionContext
		public void Schedule(IActivity activity, ExecutingAction onComplete)
		{
			_commandQueue.Enqueue(new QueueItem(activity.Execute, activity, onComplete));
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

		public T Evaluate<T>(String expression)
		{
			Console.WriteLine($"Evaluate: {expression}");
			return default;
		}

		public void Execute(String expression)
		{
			Console.WriteLine(expression);
		}

		public void BuildScript(IActivity root)
		{
		}
	}
}
