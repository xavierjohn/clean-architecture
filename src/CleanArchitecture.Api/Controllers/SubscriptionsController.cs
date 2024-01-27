using CleanArchitecture.Application.Subscriptions.Commands.CancelSubscription;
using CleanArchitecture.Application.Subscriptions.Commands.CreateSubscription;
using CleanArchitecture.Application.Subscriptions.Common;
using CleanArchitecture.Application.Subscriptions.Queries.GetSubscription;
using CleanArchitecture.Contracts.Subscriptions;

using FunctionalDdd;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using DomainSubscriptionType = CleanArchitecture.Domain.Users.SubscriptionType;
using SubscriptionType = CleanArchitecture.Contracts.Common.SubscriptionType;

namespace CleanArchitecture.Api.Controllers;

[Route("users/{userId:guid}/subscriptions")]
public class SubscriptionsController(IMediator _mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateSubscription(Guid userId, CreateSubscriptionRequest request)
    {
        if (!DomainSubscriptionType.TryFromName(request.SubscriptionType.ToString(), out var subscriptionType))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Invalid plan type");
        }

        var command = new CreateSubscriptionCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Email,
            subscriptionType);

        var result = await _mediator.Send(command);

        return result.Match(
            subscription => CreatedAtAction(
                actionName: nameof(GetSubscription),
                routeValues: new { UserId = userId },
                value: ToDto(subscription)),
            Problem);
    }

    [HttpDelete("{subscriptionId:guid}")]
    public async Task<ActionResult<FunctionalDdd.Unit>> DeleteSubscription(Guid userId, Guid subscriptionId)
    {
        var command = new CancelSubscriptionCommand(userId, subscriptionId);

        return await _mediator.Send(command)
            .FinallyAsync(
            _ => NoContent(),
            err => err.ToErrorActionResult<FunctionalDdd.Unit>(this));
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscription(Guid userId)
    {
        var query = new GetSubscriptionQuery(userId);

        var result = await _mediator.Send(query);

        return result.Match(
            user => Ok(ToDto(user)),
            Problem);
    }

    private static SubscriptionType ToDto(DomainSubscriptionType subscriptionType) =>
        subscriptionType.Name switch
        {
            nameof(DomainSubscriptionType.Basic) => SubscriptionType.Basic,
            nameof(DomainSubscriptionType.Pro) => SubscriptionType.Pro,
            _ => throw new InvalidOperationException(),
        };

    private static SubscriptionResponse ToDto(SubscriptionResult subscriptionResult) =>
        new(
            subscriptionResult.Id,
            subscriptionResult.UserId,
            ToDto(subscriptionResult.SubscriptionType));
}