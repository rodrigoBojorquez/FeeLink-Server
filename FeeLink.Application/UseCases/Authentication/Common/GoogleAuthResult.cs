namespace FeeLink.Application.UseCases.Authentication.Common;

public record GoogleAuthResult(
    string sub,
    string aud,
    string email,
    string name,
    string given_name,
    string? family_name,
    string picture);