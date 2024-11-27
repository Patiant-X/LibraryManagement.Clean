using LibraryManagement.Application.Models.Identity;

namespace LibraryManagement.Application.Contracts.Identity
{
    public interface IAuthServices
    {
        Task<AuthResponse> Login(AuthRequest request);

        Task<RegistrationResponse> Register(RegistrationRequest request);
    }
}
