using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common.Constants;
using System.Text.Json;
using Infrastructure.Extensions;

namespace Infrastructure.Identity.Authorization;
public class ClaimRequirementFilter : IAuthorizationFilter
{
    private readonly CommandCode _commandCode;
    private readonly FunctionCode _functionCode;
    public ClaimRequirementFilter(CommandCode commandCode, FunctionCode functionCode)
    {
        _commandCode = commandCode;
        _functionCode = functionCode;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionsClaim = context.HttpContext.User.Claims
            .SingleOrDefault(x => x.Type.Equals(SystemConstants.Claims.Permissions));
        
        if(permissionsClaim != null)
        {
            var permission = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);
            if(!permission.Contains(PermissionHelper.GetPermission(_functionCode, _commandCode)))
            {
                context.Result = new ForbidResult();
            }    
        }   
        else
        {
            context.Result = new ForbidResult();
        }    
    }
}
