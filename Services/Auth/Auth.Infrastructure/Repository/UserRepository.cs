using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly AuthServiceDbContext _context;

    public UserRepository(AuthServiceDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUser(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await _context.User.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ChangeRole(Guid userId, RoleEnum role)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        _context.User.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Id == id);
    }

}