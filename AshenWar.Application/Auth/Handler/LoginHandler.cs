using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Exceptions;
using MediatR;

namespace AshenWar.Application.Auth.Handler;

public record LoginRequest(string Email, string Password) : IRequest<AuthResult>;

public class LoginHandler(
    IUserRepository userRepository,
    AuthService authService) : IRequestHandler<LoginRequest, AuthResult>
{
    public async Task<AuthResult> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        // Always run BCrypt event if user not found
        // prevents timing attacks - attacker can't tell
        // if the email exists based on response time
        var hashToCheck = user?.PasswordHash ?? BCrypt.Net.BCrypt.HashPassword("dummy-prevents-timing-attack");

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, hashToCheck);
        
        if (user is null || !valid)
            throw new DomainException("Invalid email or password");

        return await authService.IssueTokenAsync(user, ct);
    }
}