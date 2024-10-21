namespace ExpenseTracker.Application.Models;

public record EmailMessage(
    string To,
    string Username,
    string Subject,
    string? FallbackUrl);
