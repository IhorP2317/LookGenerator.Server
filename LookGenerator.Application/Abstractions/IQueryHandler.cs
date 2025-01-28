using MediatR;

namespace LookGenerator.Application.Abstractions ;

    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>;  