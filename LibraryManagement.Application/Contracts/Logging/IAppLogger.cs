namespace LibraryManagement.Application.Contracts.Logging
{
    public interface IAppLogger<T>
    {
        void LogInformation(string message, params Object[] args);
        void LogWarning(string message, params Object[] args);
    }
}
