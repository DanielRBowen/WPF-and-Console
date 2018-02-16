namespace SpreadsheetGUI
{
	internal sealed class JoinSessionResponse: FailureResponse
	{
		public string Version { get; set; }

		public string Xml { get; set; }
	}
}