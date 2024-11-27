using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Models.Identity;
using LibraryManagement.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Identity.Services
{
    public class UserService : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Customer> GetCustomer(string userId)
        {
            var customer = await _userManager.FindByIdAsync(userId);
            return new Customer
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Id = customer.Id,
            };

        }

        public async Task<List<Customer>> GetCustomers()
        {
            var customer = await _userManager.GetUsersInRoleAsync("Customer");
            return customer.Select(x => new Customer
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();
        }
    }
}
