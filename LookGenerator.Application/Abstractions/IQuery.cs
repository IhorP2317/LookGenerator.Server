using MediatR;

namespace LookGenerator.Application.Abstractions ;

    public interface IQuery<out TResponse> : IRequest<TResponse>;