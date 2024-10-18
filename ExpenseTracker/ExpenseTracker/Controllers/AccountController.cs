using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Application.Services.Interfaces;
using ExpenseTracker.Application.Stores.Interfaces;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace ExpenseTracker.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IWalletStore _walletStore;

    public AccountController(
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        IEmailService emailService,
        IWalletStore walletStore)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _walletStore = walletStore;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserRequest request, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, true, false);

        if (result.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(request);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserRequest model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _walletStore.CreateDefault(user.Id);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationUrl = Url.Action(
                nameof(ConfirmEmail),           
                "Account",                
                new { email = user.Email, token },
                protocol: Request.Scheme);

            _emailService.SendConfirmation(
                user.Email,
                confirmationUrl);

            return RedirectToAction(nameof(ConfirmEmail));
        }

        AddErrors(result);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    public IActionResult ConfirmEmail()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            ViewData["ErrorMessage"] = "The email confirmation link is invalid or expired. Please request a new confirmation email.";
            return View();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            ViewData["ErrorMessage"] = "The email confirmation link is invalid or expired. Please request a new confirmation email.";
            return View();
        }

        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return BadRequest("Invalid request");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var confirmationUrl = Url.Action(
            nameof(ResetPassword),
            "Account",
            new { email = user.Email, token },
            protocol: Request.Scheme);

        _emailService.SendResetPassword(
            user.Email,
            confirmationUrl);

        return RedirectToAction(nameof(ResetSent));
    }

    public IActionResult ResetSent()
    {
        return View();
    }

    public IActionResult ResetPassword(string email, string token)
    {
        var request = new Application.Requests.Auth.ResetPasswordRequest(email, null, null, token);

        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromForm] Application.Requests.Auth.ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Invalid request");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Login));
        }

        return BadRequest("Invalid request");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
