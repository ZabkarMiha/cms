using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using CMSApi.Contracts;

namespace CMSApi.Services
{
    public static class StartupSeedService
    {
        public static async void IdentitySeed(
            UserManager<ProfileModel>? userManager,
            RoleManager<IdentityRole<Guid>>? roleManager,
            IMapper mapper
        )
        {
            if (roleManager != null)
            {
                if (!roleManager.RoleExistsAsync("Admin").Result)
                    roleManager.CreateAsync(new IdentityRole<Guid>("Admin")).Wait();
                if (!roleManager.RoleExistsAsync("Viewer").Result)
                    roleManager.CreateAsync(new IdentityRole<Guid>("Viewer")).Wait();
            }

            if (userManager != null)
            {
                if (userManager.GetUsersInRoleAsync("Admin").Result.Count() == 0)
                {
                    RegisterProfileDTO registerProfile = new RegisterProfileDTO
                    {
                        Username = "admin",
                        Password = "admin",
                        LastName = "Admin",
                        FirstName = "Admin",
                        Email = "admin@admin.com",
                        PhoneNumber = "000 000 000"
                    };
                    ProfileModel profileModel = mapper.Map<ProfileModel>(registerProfile);
                    userManager.CreateAsync(profileModel, registerProfile.Password).Wait();
                    userManager.AddToRoleAsync(profileModel, "Admin").Wait();
                }
            }
        }
    }
}
