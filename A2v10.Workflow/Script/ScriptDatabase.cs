using A2v10.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
	public class ScriptDatabase
	{
#pragma warning disable IDE1006 // Naming Styles
		public ExpandoObject loadModel(String procedure, ExpandoObject prms = null)
#pragma warning restore IDE1006 // Naming Styles
		{
			var eo = new ExpandoObject();
			eo.Set("prop", "value");
			return eo;
		}
	}
}
