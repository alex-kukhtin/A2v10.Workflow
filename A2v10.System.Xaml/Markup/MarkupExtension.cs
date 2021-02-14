﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.System.Xaml
{
	public abstract class MarkupExtension
	{
		protected MarkupExtension()
		{
		}

		public abstract Object ProvideValue(IServiceProvider serviceProvider);
	}
}