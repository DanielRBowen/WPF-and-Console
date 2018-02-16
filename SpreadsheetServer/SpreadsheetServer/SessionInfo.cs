using SS;
using System.Collections.Generic;

namespace SpreadsheetServer
{
	internal sealed class SessionInfo
	{
		public string SpreadsheetName { get; set; }

		public string Password { get; set; }

		public Spreadsheet Spreadsheet { get; set; }

		public uint SpreadsheetVersion { get; set; }

		public Stack<UnsavedChange> SpreadsheetUnsavedChanges { get; set; }

		public SortedSet<SpreadsheetServiceServer> Clients { get; set; }

		public readonly object SpreadsheetLock = new object();
	}
}