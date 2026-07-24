using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Core.Exceptions;
using Core.Requests;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Services;
using WebApi.Services;
using WebApi.Contracts;
using System.Text.Encodings.Web;

namespace WebApi.Business
{
    public interface IProfileBusinessLayer
    {
        Task<ReturnProfileDTO> GetUserInfo(string username);
        Task DeleteProfile(string username);
        Task ChangePassword(string username, ChangePasswordRequest changePasswordRequest);
        Task ForgotPassword(string email);
        Task ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task Register(RegisterProfileDTO registerProfile);
        Task ConfirmRegister(string email, string token);
        Task<object> Login(LoginRequest loginRequest);
        Task UploadProfilePicture(string username, IFormFile picture);
        Task DeleteProfilePicture(string username);
    }

    public class ProfileBusinessLayer : IProfileBusinessLayer
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ProfileModel> _userManager;
        private readonly IJWTSecurityTokenService _jWTSecurityTokenService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;

        public ProfileBusinessLayer(
            IMapper mapper,
            UserManager<ProfileModel> userManager,
            IJWTSecurityTokenService jWTSecurityTokenService,
            IEmailService emailService,
            IFileService fileService
        )
        {
            _mapper = mapper;
            _userManager = userManager;
            _jWTSecurityTokenService = jWTSecurityTokenService;
            _emailService = emailService;
            _fileService = fileService;
        }

        public async Task<ReturnProfileDTO> GetUserInfo(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            ReturnProfileDTO returnProfile = _mapper.Map<ReturnProfileDTO>(user);

            if(user.ProfilePictureUrl is not null)
                returnProfile.PictureUrl = await _fileService.RetrieveProfilePictureUrl(user.ProfilePictureUrl);

            return returnProfile;
        }

        public async Task DeleteProfile(string username)
        {
            var result = await _userManager.FindByNameAsync(username);

            if (result.ProfilePictureUrl is not null)
                await _fileService.DeleteProfilePicture(result.ProfilePictureUrl);

            await _userManager.DeleteAsync(result);
        }

        public async Task ChangePassword(string username, ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordRequest.CurrentPassword,
                changePasswordRequest.NewPassword
            );

            string errors = string.Join(", ", result.Errors.ToList().Select(e => e.Description));
            if (!string.IsNullOrWhiteSpace(errors))
                throw new ProfileException(errors);
        }

        public async Task ForgotPassword(string email)
        {
            var user =
                await _userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = $"https://some/web/page/?email={email}?token={token}";

            var emailBody =
                $"Reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> clicking here.</a> ";

            var sendEmail = await _emailService.SendEmail(
                emailBody,
                user.UserName,
                user.Email,
                "Forgot Password"
            );

            if (!sendEmail)
                throw new CouldntSendConfirmEmail();
        }

        public async Task ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);

            if (resetPasswordRequest.Password != resetPasswordRequest.ConfirmPassword)
                throw new PasswordsDontMatchException();

            var result = await _userManager.ResetPasswordAsync(
                user,
                resetPasswordRequest.Token,
                resetPasswordRequest.Password
            );

            string error = string.Join(", ", result.Errors.ToList().Select(e => e.Description));
            if (!string.IsNullOrEmpty(error))
                throw new ProfileException(error);
        }

        public async Task Register(RegisterProfileDTO registerProfileDTO)
        {
            if (
                string.IsNullOrWhiteSpace(registerProfileDTO.Email)
                || string.IsNullOrWhiteSpace(registerProfileDTO.FirstName)
                || string.IsNullOrWhiteSpace(registerProfileDTO.LastName)
                || string.IsNullOrWhiteSpace(registerProfileDTO.PhoneNumber)
                || string.IsNullOrWhiteSpace(registerProfileDTO.Password)
                || string.IsNullOrWhiteSpace(registerProfileDTO.ConfirmPassword)
            )
                throw new Exception("One or more input fields are empty.");

            if (registerProfileDTO.Password != registerProfileDTO.ConfirmPassword)
                throw new PasswordsDontMatchException();

            var profile = _mapper.Map<ProfileModel>(registerProfileDTO);

            var result = await _userManager.CreateAsync(profile, registerProfileDTO.Password);
            string errors = string.Join(", ", result.Errors.ToList().Select(e => e.Description));
            if (!string.IsNullOrWhiteSpace(errors))
                throw new ProfileException(errors);

            await _userManager.AddToRoleAsync(profile, "Viewer");

            if (registerProfileDTO.ProfilePicture is not null)
            {
                profile.ProfilePictureUrl = await _fileService.UploadProfilePicture(
                    registerProfileDTO.ProfilePicture,
                    profile.Id
                );
                await _userManager.UpdateAsync(profile);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(profile);

            var callbackUrl =
                $"https://localhost:7241/api/Profile/ConfirmRegister?email={profile.Email}&token={token}";

            var emailBody =
                $"Please confirm your email address by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> clicking here.</a> ";

            var sendEmail = await _emailService.SendEmail(
                emailBody,
                profile.UserName,
                profile.Email,
                "Confirm Email"
            );

            if (!sendEmail)
                throw new CouldntSendConfirmEmail();
        }

        public async Task ConfirmRegister(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.ConfirmEmailAsync(user, token.Replace(" ", "+"));

            string errors = string.Join(", ", result.Errors.ToList().Select(e => e.Description));
            if (!string.IsNullOrWhiteSpace(errors))
                throw new ProfileException(errors);
        }

        public async Task<object> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                    throw new EmailNotConfirmedException();

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                var token = _jWTSecurityTokenService.GetToken(authClaims);

                return new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };
            }
            else
                throw new IncorrectUserNameOrPasswordException();
        }

        public async Task UploadProfilePicture(string username, IFormFile picture)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user.ProfilePictureUrl is null)
            {
                user.ProfilePictureUrl = await _fileService.UploadProfilePicture(picture, user.Id);
                await _userManager.UpdateAsync(user);
            }
            else
                await _fileService.UploadProfilePicture(picture, user.Id);
        }

        public async Task DeleteProfilePicture(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user.ProfilePictureUrl is not null)
            {
                await _fileService.DeleteProfilePicture(user.ProfilePictureUrl);
                user.ProfilePictureUrl = null;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
