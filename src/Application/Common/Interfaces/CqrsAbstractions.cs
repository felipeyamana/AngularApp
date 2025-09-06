// Defining interfaces for commands/queries cleans up the injection of every required service instead of depending on manually registering each handler individually
// or relying on reflection to register the handlers that might end with "Handler"

namespace Application.Common.Interfaces
{
    // Command (write) marker interface
    public interface ICommandHandler<TRequest>
    {
        Task Handle(TRequest request);
    }

    // Command with result
    public interface ICommandHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    // Query (read) marker interface
    public interface IQueryHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }
}
