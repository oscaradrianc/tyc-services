using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Solg.Common.Application.Messaging;
using Solg.Common.Domain;
using Solg.Common.Presentation.ApiResults;
using Solg.Common.Presentation.Endpoints;

namespace Tyc.ws.Features.Examples;

internal sealed class SaveExample : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("examples", async (Request request, ISender sender) =>
            {
                Result<string> result = await sender.Send(new SaveExampleCommand(request.Id, request.Name));

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(ExamplesFeature.Tags);
    }

    internal sealed class Request()
    {
        public required string Id { get; init; }

        public required string Name { get; init; }
    }

    internal sealed record SaveExampleCommand(
        string Id,
        string Name) : ICommand<string>;

    internal sealed class SaveExampleValidator : AbstractValidator<SaveExampleCommand>
    {
        public SaveExampleValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
        }
    }

    internal sealed class SaveExampleCommandHandler
    : ICommandHandler<SaveExampleCommand, string>
    {
        public async Task<Result<string>> Handle(SaveExampleCommand request, CancellationToken cancellationToken)
        {
            Result<Example> result = Example.Create(
                request.Id,
                request.Name);



            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }

            await Task.Delay(100, cancellationToken);

            return result.Value.Id;
        }
    }

    internal sealed class Example : Entity
    {
        private Example()
        {
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public static Result<Example> Create(
            string id,
            string name)
        {
            var example = new Example
            {
                Id = id,
                Name = name
            };

            example.Raise(new SaveExampleDomainEvent(example.Id));

            return example;
        }
    }

    internal sealed class SaveExampleDomainEvent(string exampleId) : DomainEvent
    {
        public string ExampleId { get; init; } = exampleId;
    }

    internal static class ExampleErrors
    {
        public static Error NotFound(Guid ExampleId) =>
            Error.NotFound("Examples.NotFound", $"The example with the identifier {ExampleId} not found");

    }
}
