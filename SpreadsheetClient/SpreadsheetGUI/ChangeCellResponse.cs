using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
	internal sealed class ChangeCellResponse: FailureResponse
	{
		public string Version { get; set; }

		public bool VersionOutOfDate { get; set; }
	}
}