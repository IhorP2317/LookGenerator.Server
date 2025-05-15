using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Common;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.WebAPI.Filters;

public class EntityOwnershipAuthorizationFilter<TEntity> : IEndpointFilter
    where TEntity : BaseEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var method = httpContext.Request.Method;

        var dbContext = httpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
        var currentUser = httpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        var id = context.GetArgument<Guid>(0);

        var entity = await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
            return Results.NotFound();

        if (!Guid.TryParse(currentUser.UserId, out var userGuid))
            return Results.Unauthorized();

        var isAdmin = currentUser.UserRole == "Admin";
        var isCreator = entity.CreatedBy == userGuid;

        var isDeletingSelf = typeof(TEntity) == typeof(User) && entity.Id == userGuid;


        return method switch
        {
            var m when m == HttpMethods.Put || m == HttpMethods.Patch =>
                isAdmin || isCreator ? await next(context) : Results.Forbid(),

            var m when m == HttpMethods.Delete =>
                isAdmin
                    ? isDeletingSelf ? Results.BadRequest("Admins cannot delete themselves.") : await next(context)
                    : isCreator || isDeletingSelf ? await next(context) : Results.Forbid(),

            _ => await next(context)
        };
    }
}