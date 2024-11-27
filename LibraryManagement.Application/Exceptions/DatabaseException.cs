namespace LibraryManagement.Application.Exceptions
{
    public class DatabaseException : ApplicationException
    {
        public DatabaseException(string message, Exception innerException = null)
       : base(message, innerException)
        {
        }
    }
}
