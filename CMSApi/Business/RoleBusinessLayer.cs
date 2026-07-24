using AutoMapper;
using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace CMSApi.Business
{
    public interface IRoleBusinessLayer
    {
        Task CreateRole(string role);
        Task DeleteRole(string role);
        Task AddToRole(Guid id, string role);
        Task RemoveFromRole(Guid id, string role);
    }

    public class RoleBusinessLayer : IRoleBusinessLayer
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ProfileModel> _userManager;
        private readonly IMapper _mapper;

        public RoleBusinessLayer(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ProfileModel> userManager,
            IMapper mapper
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task CreateRole(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
                throw new RoleExistsException(role);

            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        public async Task DeleteRole(string role)
        {
            var result =
                await _roleManager.FindByNameAsync(role)
                ?? throw new RoleDoesntExistException(role);

            await _roleManager.DeleteAsync(result);
        }

        public async Task AddToRole(Guid id, string role)
        {
            var user =
                await _userManager.FindByIdAsync(id.ToString())
                ?? throw new UserNotFoundException();

            if (!await _roleManager.RoleExistsAsync(role))
                throw new RoleDoesntExistException(role);

            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task RemoveFromRole(Guid id, string role)
        {
            var user =
                await _userManager.FindByIdAsync(id.ToString())
                ?? throw new UserNotFoundException();

            if (!await _roleManager.RoleExistsAsync(role))
                throw new RoleDoesntExistException(role);

            await _userManager.RemoveFromRoleAsync(user, role);
        }
    }
}
