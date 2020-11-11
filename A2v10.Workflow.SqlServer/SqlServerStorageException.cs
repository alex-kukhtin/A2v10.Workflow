using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Workflow.SqlServer
{
	public sealed class SqlServerStorageException : Exception
	{
		public SqlServerStorageException(String message)
			:base(message)
		{
		}
	}
}
