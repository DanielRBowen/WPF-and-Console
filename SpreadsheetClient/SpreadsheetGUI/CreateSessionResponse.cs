using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
	internal sealed class CreateSessionResponse: FailureResponse
	{
		public string Password { get; set; }
	}
}