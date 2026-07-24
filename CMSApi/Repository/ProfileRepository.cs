using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSApi.Repository
{
    public interface IProfileRepository
    {
        Task<IEnumerable<ProfileModel>> GetManagedUsers(
            Guid handlerId,
            int pageNumber,
            int pageSize
        );
        Task<ProfileModel> GetManagedUserById(Guid handlerId, Guid userId);
    }

    public class ProfileRepository : IProfileRepository
    {
        private readonly CoreDbContext _cmsContext;

        public ProfileRepository(CoreDbContext cmsContext)
        {
            _cmsContext = cmsContext;
        }

        protected DbSet<ProfileModel> profileSet => _cmsContext.Set<ProfileModel>();
        protected DbSet<HandlersUsersModel> handlersUsersSet =>
            _cmsContext.Set<HandlersUsersModel>();

        public async Task<IEnumerable<ProfileModel>> GetManagedUsers(
            Guid handlerId,
            int pageNumber,
            int pageSize
        )
        {
            var profiles = await profileSet
                .Join(
                    handlersUsersSet,
                    p => p.Id,
                    hu => hu.UserId,
                    (p, hu) => new { Profile = p, HandlerUser = hu }
                )
                .Where(x => x.HandlerUser.HandlerId == handlerId)
                .Select(x => x.Profile)
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return profiles;
        }

        public async Task<ProfileModel> GetManagedUserById(Guid handlerId, Guid userId)
        {
            var profile = await profileSet
                .Join(
                    handlersUsersSet,
                    p => p.Id,
                    hu => hu.UserId,
                    (p, hu) => new { Profile = p, HandlerUser = hu }
                )
                .Where(x => x.HandlerUser.HandlerId == handlerId && x.Profile.Id == userId)
                .Select(x => x.Profile)
                .SingleOrDefaultAsync();

            return profile;
        }
    }
}
