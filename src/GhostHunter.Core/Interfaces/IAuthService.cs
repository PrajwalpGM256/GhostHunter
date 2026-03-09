using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password);
    Task<User?> LoginAsync(string email, string password);
    string GenerateToken(User user);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
}
