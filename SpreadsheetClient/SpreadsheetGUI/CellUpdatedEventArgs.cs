using System;

namespace SpreadsheetGUI
{
	internal sealed class CellUpdatedEventArgs : EventArgs
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public string Cell { get; set; }

		public string Content { get; set; }
	}
}