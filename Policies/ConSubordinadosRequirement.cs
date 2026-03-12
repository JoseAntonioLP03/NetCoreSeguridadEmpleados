using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class ConSubordinadosRequirement : AuthorizationHandler<ConSubordinadosRequirement>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ConSubordinadosRequirement requirement)
        {
            if (context.User.HasClaim(z => z.Type == ClaimTypes.NameIdentifier) == false)
            {
                context.Fail();
            }
            else
            {
                HttpContext httpContext = context.Resource as HttpContext;

                if (httpContext == null && context.Resource is AuthorizationFilterContext filterContext)
                {
                    httpContext = filterContext.HttpContext;
                }

                if (httpContext != null)
                {
                    var repo = httpContext.RequestServices.GetService<RepositoryHospital>();

                    string data = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int idEmpleado = int.Parse(data);

                    bool subs = await repo.TieneSubordinadosAsync(idEmpleado);

                    if (subs == true)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
                else
                {
                    context.Fail();
                }
            }


        }
    }
}