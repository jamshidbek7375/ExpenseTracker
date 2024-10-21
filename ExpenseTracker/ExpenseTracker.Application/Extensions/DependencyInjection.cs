using ExpenseTracker.Application.Hubs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Services.Interfaces;
using ExpenseTracker.Application.Stores;
using ExpenseTracker.Application.Stores.Interfaces;
using ExpenseTracker.Stores;
using ExpenseTracker.Stores.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryStore, CategoryStore>();
        services.AddScoped<ITransferStore, TransferStore>();
        services.AddScoped<IWalletStore, WalletStore>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSignalR();
        services.AddSingleton<NotificationHub>();

        return services;
    }
}
