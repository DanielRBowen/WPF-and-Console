namespace SpreadsheetServer
{
    internal sealed class UnsavedChange
    {
        public string CellName { get; set; }

        public string PreviousCellContents { get; set; }
    }
}