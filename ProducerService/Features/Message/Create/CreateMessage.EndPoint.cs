using Carter;
using KafkaProducer.Api.Shared.Behaviour;
using KafkaProducer.Api.Shared.Constants;
using KafkaProducer.Api.Shared.Extensions;
using MediatR;

namespace KafkaProducer.Api.Features.Message.Create
{
    public class CreateMessageEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(MessageServicesNames.Create, async (CreateMessageCommand request, ISender sender) =>
            {
                Result<Guid> result = await sender.Send(request);
                if (!result.IsSuccess)
                {
                    return result.ToProblemDetails();
                }
                return Results.Ok(result.Value);
            });
        }
    }
}
