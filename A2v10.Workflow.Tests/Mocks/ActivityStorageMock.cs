
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using A2v10.Workflow.Interfaces;

namespace A2v10.Workflow.Tests.Mocks
{
	using ExecutingAction = Func<IExecutionContext, IActivity, ValueTask>;
}
