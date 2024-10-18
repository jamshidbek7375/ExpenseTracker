using ExpenseTracker.Application.Services.Interfaces;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Configurations;
using ExpenseTracker.Infrastructure.Email;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<IImageFileRepository, ImageFileRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletShareRepository, WalletShareRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICommonRepository, CommonRepository>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddDbContext<ExpenseTrackerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ExpenseTrackerDbContextConnection")));

        services
            .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ExpenseTrackerDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
