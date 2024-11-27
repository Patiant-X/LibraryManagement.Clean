namespace LibraryManagement.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string name, object key) : base($"{name} ({key}) was not found")
        {

        }
    }
}
