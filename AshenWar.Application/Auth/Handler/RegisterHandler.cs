using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.Exceptions;
using MediatR;

namespace AshenWar.Application.Auth.Handler;

public record RegisterRequest(string Username, string Email, string Password) : IRequest<AuthResult>;

public sealed class RegisterHandler(
    IUserRepository userRepository,
    AuthService authService
    ) : IRequestHandler<RegisterRequest, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterRequest request, CancellationToken ct)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, ct))
            throw new DomainException("Email already registered.");

        if (await userRepository.ExistsByUsernameAsync(request.Username, ct))
            throw new DomainException("Username already taken");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Username, request.Email, passwordHash);

        await userRepository.SaveAsync(user, ct);

        return await authService.IssueTokenAsync(user, ct);
    }
}