using LibraryManagement.Application.Models.Identity;

namespace LibraryManagement.Application.Contracts.Identity
{
    public interface IUserServices
    {
        Task<List<Customer>> GetCustomers();

        Task<Customer> GetCustomer(string userId);
    }
}
