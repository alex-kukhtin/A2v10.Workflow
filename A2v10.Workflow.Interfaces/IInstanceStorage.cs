using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface IInstanceStorage
	{
		Task<IInstance> Load(Guid id);
		Task Save(IInstance instance);
	}
}
