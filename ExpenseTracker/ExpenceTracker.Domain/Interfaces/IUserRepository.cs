using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Domain.Interfaces;

public interface IUserRepository
{
    List<IdentityUser<Guid>> GetAll();
    IdentityUser<Guid> GetById(Guid id);
    IdentityUser<Guid>? GetByEmail(string email);
}
