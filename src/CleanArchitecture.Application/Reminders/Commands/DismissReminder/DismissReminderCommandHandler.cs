using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Users;

using MediatR;

namespace CleanArchitecture.Application.Reminders.Commands.DismissReminder;

public class DismissReminderCommandHandler(
    IUsersRepository _usersRepository)
        : IRequestHandler<DismissReminderCommand, Result<FunctionalDdd.Unit>>
{
    public async Task<Result<FunctionalDdd.Unit>> Handle(DismissReminderCommand request, CancellationToken cancellationToken)
        => await UserId.TryCreate(request.UserId)
            .BindAsync(userId => _usersRepository.GetByIdAsync(userId, cancellationToken).ToResultAsync(Error.NotFound("Reminder not found.")))
            .BindAsync(user => user.DismissReminder(request.ReminderId).Map(r => user))
            .BindAsync(user =>
            {
                _usersRepository.UpdateAsync(user, cancellationToken);
                return Result.Success();
            });
}
