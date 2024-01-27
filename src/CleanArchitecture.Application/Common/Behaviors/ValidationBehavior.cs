using FluentValidation;

using FunctionalDdd;

using MediatR;

namespace CleanArchitecture.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .Select(x => new ValidationError.ModelError(x.ErrorMessage, x.PropertyName))
            .ToList();

        return (dynamic)Result.Failure<FunctionalDdd.Unit>(Error.Validation(errors));
    }
}
