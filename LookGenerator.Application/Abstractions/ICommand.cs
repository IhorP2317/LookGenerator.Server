using MediatR;

namespace LookGenerator.Application.Abstractions ;

    public interface ICommand : IRequest;
    public interface ICommand<out TResponse> : IRequest<TResponse>;