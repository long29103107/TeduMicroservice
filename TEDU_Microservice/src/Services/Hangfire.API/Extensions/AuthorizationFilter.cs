using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Hangfire.API.Extensions;

public class AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
