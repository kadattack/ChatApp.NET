using ChatApp.Entities;

namespace ChatApp.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUsers user);
}