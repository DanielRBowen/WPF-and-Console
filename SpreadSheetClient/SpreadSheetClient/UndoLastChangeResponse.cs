namespace SpreadSheetClient
{
    internal sealed class UndoLastChangeResponse : FailureResponse
    {
        public string Version { get; set; }

        public bool VersionOutOfDate { get; set; }

        public bool NoUnsavedChanges { get; set; }

        public string Cell { get; set; }

        public string Content { get; set; }
    }
}