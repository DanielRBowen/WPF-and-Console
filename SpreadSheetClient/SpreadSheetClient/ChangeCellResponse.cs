namespace SpreadSheetClient
{
    internal sealed class ChangeCellResponse : FailureResponse
    {
        public string Version { get; set; }

        public bool VersionOutOfDate { get; set; }
    }
}