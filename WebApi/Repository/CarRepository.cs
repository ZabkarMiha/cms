using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repository
{
    public interface ICarRepository
    {
        Task<IEnumerable<CarModel>> GetUserOwnedCars(Guid id, int pageNumber, int pageSize);
        Task CreateCar(CarModel carModel);
        Task UpdateCar(CarModel carModel);
        Task<IEnumerable<CarBodyModel>> GetCarBodyModels();
        Task<IEnumerable<CarBrandModel>> GetCarBrandModels();
        Task<IEnumerable<CarEngineModel>> GetCarEngineModels();
    }

    public class CarRepository : ICarRepository
    {
        private readonly CoreDbContext _cmsContext;

        public CarRepository(CoreDbContext cmsContext)
        {
            _cmsContext = cmsContext;
        }

        protected DbSet<CarModel> carSet => _cmsContext.Set<CarModel>();
        protected DbSet<UserCarsModel> userCarsSet => _cmsContext.Set<UserCarsModel>();
        protected DbSet<CarBodyModel> carBodySet => _cmsContext.Set<CarBodyModel>();
        protected DbSet<CarBrandModel> carBrandSet => _cmsContext.Set<CarBrandModel>();
        protected DbSet<CarEngineModel> carEngineSet => _cmsContext.Set<CarEngineModel>();

        public async Task<IEnumerable<CarModel>> GetUserOwnedCars(
            Guid id,
            int pageNumber,
            int pageSize
        )
        {
            var userCarIds = await userCarsSet
                .Where(uc => uc.UserId == id)
                .Select(uc => uc.CarId)
                .ToListAsync();

            return await carSet
                .Where(c => userCarIds.Contains(c.Id))
                .Include(c => c.BodyType)
                .Include(c => c.Brand)
                .Include(c => c.EngineType)
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task CreateCar(CarModel carModel)
        {
            await carSet.AddAsync(carModel);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task UpdateCar(CarModel carModel)
        {
            carSet.Update(carModel);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CarBodyModel>> GetCarBodyModels()
        {
            return await carBodySet.ToListAsync();
        }

        public async Task<IEnumerable<CarBrandModel>> GetCarBrandModels()
        {
            return await carBrandSet.ToListAsync();
        }

        public async Task<IEnumerable<CarEngineModel>> GetCarEngineModels()
        {
            return await carEngineSet.ToListAsync();
        }
    }
}
