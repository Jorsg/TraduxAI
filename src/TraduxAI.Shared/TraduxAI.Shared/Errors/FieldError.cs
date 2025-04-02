﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Errors
{
	public class FieldError
	{
		public FieldError(string messageValue)
		{
			Message = messageValue;
		}

		public string Message { get; set; }
	}
}
