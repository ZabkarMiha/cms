using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Core.Exceptions;
using CMSApi.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMSApi.Repository;
using Microsoft.EntityFrameworkCore;
using Core.Services;
using Core.Models;
using Core.Requests;

namespace CMSApi.Business
{
    public interface IProfileBusinessLayer
    {
        Task<IEnumerable<ReturnProfileDTO>> GetProfiles(
            string username,
            PaginationRequest paginationRequest
        );
        Task<ReturnProfileDTO> GetProfileById(Guid id, string username);
        Task DeleteProfile(Guid id, string username);
        Task<object> Login(LoginRequest loginRequest);
    }

    public class ProfileBusinessLayer : IProfileBusinessLayer
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ProfileModel> _userManager;
        private readonly SignInManager<ProfileModel> _signInManager;
        private readonly IProfileRepository _profileRepository;
        private readonly IJWTSecurityTokenService _jWTSecurityTokenService;
        private readonly IFileService _fileService;

        public ProfileBusinessLayer(
            IMapper mapper,
            UserManager<ProfileModel> userManager,
            SignInManager<ProfileModel> signInManager,
            IProfileRepository profileRepository,
            IJWTSecurityTokenService jWTSecurityTokenService,
            IFileService fileService
        )
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepository = profileRepository;
            _jWTSecurityTokenService = jWTSecurityTokenService;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ReturnProfileDTO>> GetProfiles(
            string username,
            PaginationRequest paginationRequest
        )
        {
            var manager = await _userManager.FindByNameAsync(username);
            IEnumerable<ProfileModel> users;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
            {
                users = await _profileRepository.GetManagedUsers(
                    manager.Id,
                    paginationRequest.PageNumber,
                    paginationRequest.PageSize
                );
            }
            else
                users = await _userManager.Users
                    .OrderBy(u => u.Id)
                    .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                    .Take(paginationRequest.PageSize)
                    .ToListAsync();

            var returnUserTasks = users.Select(async user =>
            {
                ReturnProfileDTO returnUser = _mapper.Map<ReturnProfileDTO>(user);
                if (user.ProfilePictureUrl is not null)
                {
                    returnUser.PictureUrl = await _fileService.RetrieveProfilePictureUrl(
                        user.ProfilePictureUrl
                    );
                }
                return returnUser;
            });

            return await Task.WhenAll(returnUserTasks);
        }

        public async Task<ReturnProfileDTO> GetProfileById(Guid id, string username)
        {
            var manager = await _userManager.FindByNameAsync(username);
            ProfileModel? user;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                user = await _profileRepository.GetManagedUserById(manager.Id, id);
            else
                user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                throw new UserNotFoundException();

            var returnUser = _mapper.Map<ReturnProfileDTO>(user);

            if(user.ProfilePictureUrl is not null)
                returnUser.PictureUrl = await _fileService.RetrieveProfilePictureUrl(user.ProfilePictureUrl);

            return returnUser;
        }

        public async Task DeleteProfile(Guid id, string username)
        {
            var manager = await _userManager.FindByNameAsync(username);
            ProfileModel? user = null;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                user = await _profileRepository.GetManagedUserById(manager.Id, id);
            else
                user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                throw new UserNotFoundException();

            if (user.ProfilePictureUrl is not null)
                await _fileService.DeleteProfilePicture(user.ProfilePictureUrl);

            await _userManager.DeleteAsync(user);
        }

        public async Task<object> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
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
    }
}
