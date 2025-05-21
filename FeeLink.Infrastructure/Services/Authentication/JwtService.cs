using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Constants;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FeeLink.Infrastructure.Services.Authentication;

public class JwtService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly FeeLinkDbContext _context;

    public JwtService(IRoleRepository roleRepository, IConfiguration config, FeeLinkDbContext context)
    {
        _config = config;
        _context = context;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new("jti", Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(FeeLinkConstants.RoleClaim, user.Role.Name),
            new("userName", $"{user.Name} {user.LastName}"),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Authentication:Key")!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Authentication:JwtExpireMinutes")),
            Issuer = _config.GetValue<string>("Authentication:Issuer")!,
            Audience = _config.GetValue<string>("Authentication:Audience")!,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        var existToken = await _context.RefreshTokens.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

         return existToken is not null && existToken.Expires > DateTime.UtcNow;
    }

    public async Task StoreRefreshTokenAsync(string refreshToken, Guid userId)
    {
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (existingToken is not null)
        {
            // Actualizar el token existente
            existingToken.Token = refreshToken;
            existingToken.Expires =
                DateTime.UtcNow.AddDays(_config.GetValue<int>("Authentication:RefreshTokenExpireDays"));

            _context.RefreshTokens.Update(existingToken);
        }
        else
        {
            // Insertar un nuevo token si no existe
            var entity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("Authentication:RefreshTokenExpireDays"))
            };

            await _context.RefreshTokens.AddAsync(entity);
        }

        await _context.SaveChangesAsync();
    }


    public async Task<ErrorOr<AuthResult>> RefreshToken(string refreshToken)
    {
        var existToken = _context.RefreshTokens.Include(r => r.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefault(r => r.Token == refreshToken);

        if (existToken is null || existToken.Expires < DateTime.UtcNow)
            return Errors.Authentication.InvalidRefreshToken;

        existToken.Token = GenerateRefreshToken();
        existToken.Expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("Authentication:RefreshTokenExpireDays"));

        _context.RefreshTokens.Update(existToken);
        await _context.SaveChangesAsync();

        var token = await GenerateTokenAsync(existToken.User);
        var user = existToken.User;

        return new AuthResult(Id: user.Id, AccessToken: token, RefreshToken: existToken.Token, Email: user.Email,
            GoogleAuth: !string.IsNullOrEmpty(user.GoogleId), Name: user.Name, LastName: user.LastName,
            Picture: user.Picture);
    }

    public async Task DeleteRefreshTokenAsync(string refreshToken)
    {
        var existToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (existToken is not null)
        {
            _context.RefreshTokens.Remove(existToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> GetRefreshTokenAsync(Guid userId)
    {
        var token = await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .Select(r => r.Token)
            .FirstOrDefaultAsync();

        return token;
    }
}