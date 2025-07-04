using CollaborativeOffice.Domain;

namespace CollaborativeOffice.IdentityService.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task AddUserAsync(User user);
    Task<bool> SaveChangesAsync();
}