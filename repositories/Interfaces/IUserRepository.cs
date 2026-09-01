using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<bool> EmailExistsAsync(
        string email,
        int? excludedUserId = null
    );

    Task AddAsync(User user);

    Task SaveChangesAsync();
}
