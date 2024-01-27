using CleanArchitecture.Application.Common.Interfaces;

using FunctionalDdd;

using MediatR;

namespace CleanArchitecture.Application.Reminders.Commands.DismissReminder;

public class DismissReminderCommandHandler(
    IUsersRepository _usersRepository)
        : IRequestHandler<DismissReminderCommand, Result<FunctionalDdd.Unit>>
{
    public async Task<Result<FunctionalDdd.Unit>> Handle(DismissReminderCommand request, CancellationToken cancellationToken) =>
        await _usersRepository.GetByIdAsync(request.UserId, cancellationToken)
            .ToResultAsync(Error.NotFound("Reminder not found"))
            .BindAsync(user => user.DismissReminder(request.ReminderId).Map(r => user))
            .TapAsync(user => _usersRepository.UpdateAsync(user, cancellationToken))
            .MapAsync(r => default(FunctionalDdd.Unit));
}
