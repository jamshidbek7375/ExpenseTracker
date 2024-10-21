using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ExpenseTrackerDbContext _context;

    public UserRepository(ExpenseTrackerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public List<IdentityUser<Guid>> GetAll()
    {
        var users = _context.Users.ToList();

        return users;
    }

    public IdentityUser<Guid> GetById(Guid id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        // TODO, throw in application logic when it returns null.
        // Remove it from repository
        if (user is null)
        {
            throw new EntityNotFoundException($"User with id: {id} is not found");
        }

        return user;
    }

    public IdentityUser<Guid>? GetByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email);

        return user;
    }
}
