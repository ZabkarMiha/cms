using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repository
{
    public interface IAssignRepository
    {
        Task AssignCarToUser(CarModel CarId, ProfileModel UserId);
    }

    public class AssignRepository : IAssignRepository
    {
        private readonly CoreDbContext _cmsContext;

        public AssignRepository(CoreDbContext cmsContext)
        {
            _cmsContext = cmsContext;
        }

        protected DbSet<UserCarsModel> userCarsSet => _cmsContext.Set<UserCarsModel>();

        public async Task AssignCarToUser(CarModel Car, ProfileModel User)
        {
            var userCar = new UserCarsModel { UserId = User.Id, CarId = Car.Id };

            await userCarsSet.AddAsync(userCar);

            await _cmsContext.SaveChangesAsync();
        }
    }
}
