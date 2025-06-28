using MediatR;

namespace Reports.Common.Abstractions.Mediator
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        //Task<ICollection<TableViewDto<GetAllAuctionViewModel>>> Handle(GetAllAuctionCommand request, CancellationToken cancellationToken);
    }
}
