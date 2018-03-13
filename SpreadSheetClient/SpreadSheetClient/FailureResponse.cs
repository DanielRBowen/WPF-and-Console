namespace SpreadSheetClient
{
    internal abstract class FailureResponse
    {
        public string Name { get; set; }

        public string ErrorMessage { get; set; }
    }
}