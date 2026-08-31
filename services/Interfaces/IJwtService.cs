using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IJwtService
{
    string CreateAccessToken(User user);
    RefreshToken CreateRefreshToken(User user);
}
