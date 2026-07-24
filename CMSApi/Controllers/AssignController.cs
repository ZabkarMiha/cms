using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMSApi.Business;

namespace CMSApi.Controllers
{
    [Authorize(Roles = "Admin, Handler")]
    [ApiController]
    [Route("api/[controller]")]
    public class AssignController : ControllerBase
    {
        private readonly IAssignBusinessLayer _assignBusinessLayer;

        public AssignController(IAssignBusinessLayer assignBusinessLayer)
        {
            _assignBusinessLayer = assignBusinessLayer;
        }

        [Authorize(Roles = "Admin")]
        [Route("AssignUserToHandler/{UserId}/{HandlerId}")]
        [HttpPost]
        public async Task<IActionResult> AssignUserToHandlerAsync(Guid UserId, Guid HandlerId)
        {
            await _assignBusinessLayer.AssignUserToHandler(UserId, HandlerId);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("AssignCarToHandler/{CarId}/{HandlerId}")]
        [HttpPost]
        public async Task<IActionResult> AssignCarToHandlerAsync(Guid CarId, Guid HandlerId)
        {
            await _assignBusinessLayer.AssignCarToHandler(CarId, HandlerId);
            return Ok();
        }

        [Route("AssignCarToUser/{CarId}/{UserId}")]
        [HttpPost]
        public async Task<IActionResult> AssignCarToUserAsync(Guid CarId, Guid UserId)
        {
            await _assignBusinessLayer.AssignCarToUser(CarId, UserId, User.Identity.Name);
            return Ok();
        }

        [Route("UnassignCarFromUser/{CarId}/{UserId}")]
        [HttpPost]
        public async Task<IActionResult> UnassignCarFromUserAsync(Guid CarId, Guid UserId)
        {
            await _assignBusinessLayer.UnassignCarFromUser(CarId, UserId, User.Identity.Name);
            return Ok();
        }
    }
}
