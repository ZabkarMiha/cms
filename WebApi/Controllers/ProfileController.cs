using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Business;
using Core.Requests;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin, Handler, Viewer")]
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
        public async Task<object> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            return await _profileBusinessLayer.Login(loginRequest);
        }

        [Route("GetUserInfo")]
        [HttpGet]
        public async Task<ReturnProfileDTO> GetUserInfoAsync()
        {
            return await _profileBusinessLayer.GetUserInfo(User.Identity.Name);
        }

        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(
            [FromForm] RegisterProfileDTO registerProfile
        )
        {
            await _profileBusinessLayer.Register(registerProfile);
            return Ok();
        }

        [AllowAnonymous]
        [Route("ConfirmRegister")]
        [HttpGet]
        public async Task<IActionResult> ConfirmRegisterAsync(string email, string token)
        {
            await _profileBusinessLayer.ConfirmRegister(email, token);
            return Ok();
        }

        [Route("DeleteProfile")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfileAsync()
        {
            await _profileBusinessLayer.DeleteProfile(User.Identity.Name);
            return Ok();
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ChangePasswordRequest changePasswordRequest
        )
        {
            await _profileBusinessLayer.ChangePassword(User.Identity.Name, changePasswordRequest);
            return Ok();
        }

        [AllowAnonymous]
        [Route("ForgotPassword/{email}")]
        [HttpGet]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            await _profileBusinessLayer.ForgotPassword(email);
            return Ok();
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(
            [FromBody] ResetPasswordRequest resetPasswordRequest
        )
        {
            await _profileBusinessLayer.ResetPassword(resetPasswordRequest);
            return Ok();
        }

        [Route("UploadProfilePicture")]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePictureAsync(IFormFile picture)
        {
            await _profileBusinessLayer.UploadProfilePicture(User.Identity.Name, picture);
            return Ok();
        }

        [Route("DeleteProfilePicture")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfilePictureAsync(){
            await _profileBusinessLayer.DeleteProfilePicture(User.Identity.Name);
            return Ok();
        }
    }
}
