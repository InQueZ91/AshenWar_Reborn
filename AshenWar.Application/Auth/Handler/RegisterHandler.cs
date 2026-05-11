using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.ValueObjects;
using Domain.Entities.Users;
using Domain.Exceptions;
using MediatR;

namespace Application.Auth.Handler;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<AuthResult>;

public sealed class RegisterHandler(
    IUserRepository userRepository,
    AuthService authService
    ) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand command, CancellationToken ct)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, ct))
            throw new DomainException("Email already registered.");

        if (await userRepository.ExistsByUsernameAsync(command.Username, ct))
            throw new DomainException("Username already taken");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var user = User.Create(command.Username, command.Email, passwordHash);

        await userRepository.SaveAsync(user, ct);

        return await authService.IssueTokenAsync(user, ct);
    }
}