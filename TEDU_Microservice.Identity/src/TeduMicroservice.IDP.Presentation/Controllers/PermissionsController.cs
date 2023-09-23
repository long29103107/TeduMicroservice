using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TeduMicroservice.IDP.Infrastructure.Common.Repositories;
using TeduMicroservice.IDP.Infrastructure.Entities;
using TeduMicroservice.IDP.Infrastructure.ViewModels;

namespace TeduMicroservice.IDP.Presentation.Controllers;
[Route("api/roles/{roleId}/[controller]")]
[ApiController]
[Authorize("Bearer")]
public class PermissionsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    public PermissionsController(IRepositoryManager repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult>GetPermissions(string roleId)
    {
        var result = await _repository.Permission.GetPermissionByRole(roleId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] PermissionAddModel model)
    {
        var result = await _repository.Permission.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
    }

    [HttpPost("update-permissions")]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdatePermissions(string roleId, IEnumerable<PermissionAddModel> permisstionCollection)
    {
        await _repository.Permission.UpdatePermission(roleId, permisstionCollection);
        return NoContent();
    }

    [HttpDelete("function/{function}/command/{command}")]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePermission(string roleId, [Required] string function, [Required] string command)
    {
        await _repository.Permission.DeletePermission(roleId, function, command);
        return NoContent();
    }
}
