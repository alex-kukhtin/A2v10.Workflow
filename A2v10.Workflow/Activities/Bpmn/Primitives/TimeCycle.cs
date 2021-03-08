using A2v10.System.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Bpmn
{
	public class TimeCycle : TimeBase
	{
		public override Boolean CanRepeat => true;
		public override DateTime NextTriggerTime => DateTime.UtcNow + TimeSpan.Parse(Expression);
	}
}
