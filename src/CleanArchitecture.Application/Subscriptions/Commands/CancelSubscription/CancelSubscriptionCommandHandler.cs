using CleanArchitecture.Application.Common.Interfaces;

using FunctionalDdd;

using MediatR;

namespace CleanArchitecture.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler(IUsersRepository _usersRepository)
    : IRequestHandler<CancelSubscriptionCommand, Result<FunctionalDdd.Unit>>
{
    public async Task<Result<FunctionalDdd.Unit>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken) =>
        await _usersRepository.GetByIdAsync(request.UserId, cancellationToken)
            .ToResultAsync(Error.NotFound("User not found"))
            .BindAsync(user => user.CancelSubscription(request.SubscriptionId).Map(r => user))
            .TapAsync(user => _usersRepository.UpdateAsync(user, cancellationToken))
            .MapAsync(r => default(FunctionalDdd.Unit));
}
