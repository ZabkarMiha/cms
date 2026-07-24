using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSApi.Repository
{
    public interface IAssignRepository
    {
        Task AssignUserToHandler(ProfileModel User, ProfileModel Handler);
        Task AssignCarToHandler(CarModel Car, ProfileModel Handler);
        Task AssignCarToUser(CarModel CarId, ProfileModel UserId);
        Task UnassignCarFromUser(CarModel Car, ProfileModel User);
        Task<bool> HandlerManagesUser(Guid HandlerId, Guid UserId);
        Task<bool> HandlerManagesCar(Guid HandlerId, Guid CarId);
    }

    public class AssignRepository : IAssignRepository
    {
        private readonly CoreDbContext _cmsContext;

        public AssignRepository(CoreDbContext cmsContext)
        {
            _cmsContext = cmsContext;
        }

        protected DbSet<HandlersUsersModel> handlersUsersSet =>
            _cmsContext.Set<HandlersUsersModel>();
        protected DbSet<HandlersCarsModel> handlersCarsSet => _cmsContext.Set<HandlersCarsModel>();
        protected DbSet<UserCarsModel> userCarsSet => _cmsContext.Set<UserCarsModel>();

        public async Task AssignUserToHandler(ProfileModel User, ProfileModel Handler)
        {
            var userHandler = new HandlersUsersModel { UserId = User.Id, HandlerId = Handler.Id };

            await handlersUsersSet.AddAsync(userHandler);

            await _cmsContext.SaveChangesAsync();
        }

        public async Task AssignCarToHandler(CarModel Car, ProfileModel Handler)
        {
            var carHandler = new HandlersCarsModel { CarId = Car.Id, HandlerId = Handler.Id };

            await handlersCarsSet.AddAsync(carHandler);

            await _cmsContext.SaveChangesAsync();
        }

        public async Task AssignCarToUser(CarModel Car, ProfileModel User)
        {
            var userCar = new UserCarsModel { UserId = User.Id, CarId = Car.Id };

            await userCarsSet.AddAsync(userCar);

            await _cmsContext.SaveChangesAsync();
        }

        public async Task UnassignCarFromUser(CarModel Car, ProfileModel User)
        {
            var userCar = new UserCarsModel { UserId = User.Id, CarId = Car.Id };

            _cmsContext.Remove(userCar);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task<bool> HandlerManagesUser(Guid HandlerId, Guid UserId)
        {
            var result = await handlersUsersSet.FindAsync(UserId, HandlerId);
            if (result == null)
                return false;
            else
                return true;
        }

        public async Task<bool> HandlerManagesCar(Guid HandlerId, Guid CarId)
        {
            var result = await handlersCarsSet.FindAsync(CarId, HandlerId);
            if (result == null)
                return false;
            else
                return true;
        }
    }
}
