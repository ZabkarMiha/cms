using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMSApi.Business;

namespace CMSApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleBusinessLayer _roleBusinessLayer;

        public RoleController(IRoleBusinessLayer roleBusinessLayer)
        {
            _roleBusinessLayer = roleBusinessLayer;
        }

        [Route("CreateRole/{role}")]
        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(string role)
        {
            await _roleBusinessLayer.CreateRole(role);
            return Ok();
        }

        [Route("DeleteRole/{role}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoleAsync(string role)
        {
            await _roleBusinessLayer.DeleteRole(role);
            return Ok();
        }

        [Route("AddToRole/{id:Guid}/{role}")]
        [HttpPut]
        public async Task<IActionResult> AddToRoleAsync(Guid id, string role)
        {
            await _roleBusinessLayer.AddToRole(id, role);
            return Ok();
        }

        [Route("AddToRole/{id:Guid}/{role}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveFromRoleAsync(Guid id, string role)
        {
            await _roleBusinessLayer.RemoveFromRole(id, role);
            return Ok();
        }
    }
}
