using Auth.Domain.Entities;
using Auth.Domain.Enums;

namespace Auth.Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User> GetUserByEmail(string email);
    Task<bool> ExistsByEmail(string email);
    Task<bool> ChangeRole(Guid userId, RoleEnum role);
    Task<User?> GetUserById(Guid id);
    Task<List<User>> GetUsersByRole(RoleEnum role);
}