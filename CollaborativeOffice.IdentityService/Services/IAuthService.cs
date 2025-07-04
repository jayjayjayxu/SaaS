using CollaborativeOffice.Domain;

namespace CollaborativeOffice.IdentityService.Services;

// Defines the contract for authentication-related business logic.
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <returns>A tuple indicating success and an error message if failed.</returns>
    Task<(bool Succeeded, string? Error)> RegisterAsync(string username, string password);

    /// <summary>
    /// Attempts to log in a user.
    /// </summary>
    /// <returns>A tuple containing the JWT token on success, or an error message on failure.</returns>
    Task<(string? Token, string? Error)> LoginAsync(string username, string password);
}