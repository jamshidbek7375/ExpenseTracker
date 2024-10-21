using ExpenseTracker.Application.Requests.WalletShare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Filters;

public class CreateWalletShareFilter : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;

        var walletIdValue = request.Form["WalletId"];
        var usersToShareJson = request.Form["UsersToShareJson"].ToString();

        if (!int.TryParse(walletIdValue, out int walletId) || string.IsNullOrEmpty(usersToShareJson))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var usersToShare = JsonConvert.DeserializeObject<List<string>>(usersToShareJson);

        if (usersToShare is null || usersToShare.Count < 1)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var emailValidator = new EmailAddressAttribute();

        foreach (var email in usersToShare)
        {
            if (!emailValidator.IsValid(email))
            {
                bindingContext.ModelState.AddModelError("UsersToShare", $"Invalid email address: {email}");
            }
        }

        if (!bindingContext.ModelState.IsValid)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var model = new CreateWalletShareRequest(
            Guid.Empty,
            walletId,
            null,
            usersToShare);

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}

public class CreateWalletShareRequestBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        // Check if the model is CreateWalletShareRequest
        if (context.Metadata.ModelType == typeof(CreateWalletShareRequest))
        {
            return new BinderTypeModelBinder(typeof(CreateWalletShareFilter));
        }

        return null;
    }
}
