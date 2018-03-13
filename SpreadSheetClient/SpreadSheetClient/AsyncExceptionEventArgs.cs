using System;

namespace SpreadSheetClient
{
    internal sealed class AsyncExceptionEventArgs : EventArgs
    {
        private readonly Exception myException;
        public Exception Exception
        {
            get
            {
                return myException;
            }
        }

        public AsyncExceptionEventArgs(Exception ex)
        {
            myException = ex;
        }
    }
}