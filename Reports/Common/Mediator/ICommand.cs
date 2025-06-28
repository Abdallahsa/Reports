using MediatR;

namespace Reports.Common.Abstractions.Mediator
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}
