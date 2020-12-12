using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.Interfaces
{
	public interface ISerializer
	{
		ExpandoObject Deserialize(String text);
		String Serialize(ExpandoObject obj);

		IActivity DeserializeActitity(String text, String format);
		String SerializeActitity(IActivity activity, String format);
	}
}
