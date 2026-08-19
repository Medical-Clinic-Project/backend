using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<bool> EmailExistsAsync(string email);

    Task AddAsync(User user);

    Task<RefreshToken?> GetRefreshTokenAsync(string token);

    Task AddRefreshTokenAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
}
