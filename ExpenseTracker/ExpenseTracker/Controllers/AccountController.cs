using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Application.Services.Interfaces;
using ExpenseTracker.Application.Stores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MimeKit;
using UAParser;

namespace ExpenseTracker.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IWalletStore _walletStore;
    private readonly IHttpContextAccessor _contextAccessor;

    public AccountController(
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        IEmailService emailService,
        IWalletStore walletStore,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _walletStore = walletStore;
        _contextAccessor = contextAccessor;
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
    public async Task<IActionResult> Register(RegisterUserRequest request, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), UserName = model.UserName, Email = model.Email };
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
            
            var emailMessage = new EmailMessage(request.Email, request.Email, "Email Confirmation", confirmationUrl);
            var userAgent = _contextAccessor.HttpContext?.Request?.Headers?.UserAgent;
            var agent = Parser.GetDefault().Parse(userAgent);
            var userInfo = new UserInfo(agent.UA.ToString(), agent.OS.ToString());

            _emailService.SendEmailConfirmation(emailMessage, userInfo);

            return RedirectToAction(nameof(RegisterConfirmation));
        }

        AddErrors(result);

        return View(request);
    }

    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    public IActionResult ResendConfirmation()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid request");
        }

        var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Invalid request");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationUrl = Url.Action(
            nameof(EmailConfirmed),
            "Account",
            new { email = user.Email, token },
            protocol: Request.Scheme);

        var emailMessage = new EmailMessage(request.Email, request.Email, "Email Confirmation", confirmationUrl);
        var userAgent = _contextAccessor.HttpContext?.Request?.Headers?.UserAgent;
        var agent = Parser.GetDefault().Parse(userAgent);
        var userInfo = new UserInfo(agent.UA.ToString(), agent.OS.ToString());

        _emailService.SendEmailConfirmation(emailMessage, userInfo);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> EmailConfirmed(string email, string token)
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
            ViewBag.Email = email;
            return View(new ResendConfirmationRequest(email));
        }

        var actionUrl = Url.Action(
                nameof(Login),
                "Account",
                new { email = user.Email, token },
                protocol: Request.Scheme);
        var emailMessage = new EmailMessage(email, email, "Welcome!", actionUrl);
        _emailService.SendWelcome(emailMessage);

        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(Application.Requests.Auth.ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Invalid request");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var confirmationUrl = Url.Action(
            nameof(PasswordReset),
            "Account",
            new { email = user.Email, token },
            protocol: Request.Scheme);

        var emailMessage = new EmailMessage(request.Email, request.Email, "Password reset", confirmationUrl);
        var userAgent = _contextAccessor.HttpContext?.Request?.Headers?.UserAgent;
        var agent = Parser.GetDefault().Parse(userAgent);
        var userInfo = new UserInfo(agent.UA.ToString(), agent.OS.ToString());

        _emailService.SendResetPassword(emailMessage, userInfo);


        return RedirectToAction(nameof(PasswordResetConfirmation));
    }

    public IActionResult PasswordResetConfirmation()
    {
        return View();
    }

    public IActionResult PasswordReset(string email, string token)
    {
        var request = new ResetPasswordRequest(email, null, null, token);

        return View(request);
    }

    [HttpPost]

    public async Task<IActionResult> PasswordReset([FromForm] Application.Requests.Auth.ResetPasswordRequest request)

    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Invalid request");
        }

        if (!user.EmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);
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
