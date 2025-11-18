using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeesManagementSystem.Services.Interfaces;

namespace EmployeesManagementSystem.Services;

public class LoginService(AppDbContext context, IConfiguration configuration) : ILoginService
{
    public async Task<string> LoginAsync(UserLogin request)
    {
        var user = await context.Users
            .Include(u => u.UserDepartmentRoles)
                .ThenInclude(udr => udr.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
            return null;

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password ?? string.Empty, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = CreateToken(user);

        return token;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        // Add all user roles to claims
        foreach (var userDepartmentRole in user.UserDepartmentRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userDepartmentRole.Role.Name));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}