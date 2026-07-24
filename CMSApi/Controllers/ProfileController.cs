using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMSApi.Contracts;
using CMSApi.Business;
using Core.Requests;

namespace CMSApi.Controllers
{
    [Authorize(Roles = "Admin, Handler")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileBusinessLayer _profileBusinessLayer;

        public ProfileController(IProfileBusinessLayer profileBusinessLayer)
        {
            _profileBusinessLayer = profileBusinessLayer;
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<object> Login(LoginRequest loginRequest)
        {
            return await _profileBusinessLayer.Login(loginRequest);
        }

        [Route("GetProfiles")]
        [HttpGet]
        public async Task<IEnumerable<ReturnProfileDTO>> GetProfilesAsync(
            [FromQuery] PaginationRequest paginationRequest
        )
        {
            return await _profileBusinessLayer.GetProfiles(User.Identity.Name, paginationRequest);
        }

        [Route("GetProfileById/{id:Guid}")]
        [HttpGet]
        public async Task<ReturnProfileDTO> GetProfileByIdAsync(Guid id)
        {
            return await _profileBusinessLayer.GetProfileById(id, User.Identity.Name);
        }

        [Route("DeleteProfile/{id:Guid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfileAsync(Guid id)
        {
            await _profileBusinessLayer.DeleteProfile(id, User.Identity.Name);
            return Ok();
        }
    }
}
