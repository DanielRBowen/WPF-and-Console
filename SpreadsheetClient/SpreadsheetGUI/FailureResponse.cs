using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
	internal abstract class FailureResponse
	{
		public string Name { get; set; }

		public string ErrorMessage { get; set; }
	}
}