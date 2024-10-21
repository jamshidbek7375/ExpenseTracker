namespace ExpenseTracker.Domain.Exceptions
{
    public class ShareNotAllowedException : ApplicationException
    {
        public ShareNotAllowedException() { }
        public ShareNotAllowedException(string message) : base(message) { }
    }
}
