using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSApi.Repository
{
    public interface ICarRepository
    {
        Task<IEnumerable<CarModel>> GetCars(int pageNumber, int pageSize);
        Task<IEnumerable<CarModel>> GetManagedCars(Guid handlerId, int pageNumber, int pageSize);
        Task<CarModel> GetCarById(Guid id);
        Task<CarModel> GetManagedCarById(Guid handlerId, Guid carId);
        Task DeleteCar(CarModel carModel);
        Task CreateCar(CarModel carModel);
        Task UpdateCar(CarModel carModel);
        Task CreateCarBrand(CarBrandModel brand);
        Task CreateCarEngineType(CarEngineModel engineType);
        Task CreateCarBodyType(CarBodyModel bodyType);
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
        protected DbSet<HandlersCarsModel> handlersCarsSet => _cmsContext.Set<HandlersCarsModel>();
        protected DbSet<CarBodyModel> carBodySet => _cmsContext.Set<CarBodyModel>();
        protected DbSet<CarBrandModel> carBrandSet => _cmsContext.Set<CarBrandModel>();
        protected DbSet<CarEngineModel> carEngineSet => _cmsContext.Set<CarEngineModel>();

        public async Task<IEnumerable<CarModel>> GetCars(int pageNumber, int pageSize)
        {
            return await carSet
                .Include(c => c.BodyType)
                .Include(c => c.Brand)
                .Include(c => c.EngineType)
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<CarModel>> GetManagedCars(
            Guid handlerId,
            int pageNumber,
            int pageSize
        )
        {
            var cars = await carSet
                .Join(
                    handlersCarsSet,
                    c => c.Id,
                    hc => hc.CarId,
                    (c, hc) => new { Car = c, HandlerCar = hc }
                )
                .Where(x => x.HandlerCar.HandlerId == handlerId)
                .Select(x => x.Car)
                .Include(c => c.BodyType)
                .Include(c => c.Brand)
                .Include(c => c.EngineType)
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return cars;
        }

        public async Task<CarModel> GetCarById(Guid id)
        {
            return await carSet
                .Include(c => c.BodyType)
                .Include(c => c.Brand)
                .Include(c => c.EngineType)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CarModel> GetManagedCarById(Guid handlerId, Guid carId)
        {
            var car = await carSet
                .Join(
                    handlersCarsSet,
                    c => c.Id,
                    hc => hc.CarId,
                    (c, hc) => new { Car = c, HandlerCar = hc }
                )
                .Where(x => x.HandlerCar.HandlerId == handlerId && x.Car.Id == carId)
                .Select(x => x.Car)
                .Include(c => c.BodyType)
                .Include(c => c.Brand)
                .Include(c => c.EngineType)
                .SingleOrDefaultAsync();

            return car;
        }

        public async Task CreateCar(CarModel carModel)
        {
            await carSet.AddAsync(carModel);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task DeleteCar(CarModel carModel)
        {
            carSet.Remove(carModel);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task UpdateCar(CarModel carModel)
        {
            carSet.Update(carModel);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task CreateCarBrand(CarBrandModel brand)
        {
            await carBrandSet.AddAsync(brand);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task CreateCarEngineType(CarEngineModel engineType)
        {
            await carEngineSet.AddAsync(engineType);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task CreateCarBodyType(CarBodyModel bodyType)
        {
            await carBodySet.AddAsync(bodyType);
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
