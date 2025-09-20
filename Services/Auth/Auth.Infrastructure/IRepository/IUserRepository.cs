using Auth.Domain.Entities;

namespace Auth.Infrastructure.IRepository;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User> GetUserByEmail(string email);
    Task<bool> ExistsByEmail(string email);
}