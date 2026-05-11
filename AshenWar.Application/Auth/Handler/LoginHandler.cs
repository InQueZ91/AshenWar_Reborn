using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.ValueObjects;
using Domain.Exceptions;
using MediatR;

namespace Application.Auth.Handler;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

public class LoginHandler(
    IUserRepository userRepository,
    AuthService authService) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
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