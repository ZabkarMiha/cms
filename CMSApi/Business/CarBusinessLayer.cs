using AutoMapper;
using CMSApi.Repository;
using Core.Requests;
using Core.Models;
using CMSApi.Contracts;
using Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using CMSApi.Helpers;
using Core.Services;

namespace CMSApi.Business
{
    public interface ICarBusinessLayer
    {
        Task<IEnumerable<ReturnCarDTO>> GetCars(
            string username,
            PaginationRequest paginationRequest
        );
        Task<ReturnCarDTO> GetCarById(Guid id, string username);
        Task CreateCar(CreateCarDTO createCarDTO, string username);
        Task DeleteCar(Guid id, string username);
        Task UpdateCar(Guid id, UpdateCarDTO updateCarDTO, string username);
        Task CreateCarBrand(string brand);
        Task CreateCarEngineType(string engineType);
        Task CreateCarBodyType(string bodyType);
        Task<IEnumerable<CarBodyRequest>> GetCarBodyTypes();
        Task<IEnumerable<CarBrandRequest>> GetCarBrands();
        Task<IEnumerable<CarEngineRequest>> GetCarEngineTypes();
    }

    public class CarBusinessLayer : ICarBusinessLayer
    {
        private readonly ICarRepository _carRepository;
        private readonly IAssignRepository _assignRepository;
        private readonly UserManager<ProfileModel> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CarBusinessLayer(
            ICarRepository carRepository,
            IAssignRepository assignRepository,
            UserManager<ProfileModel> userManager,
            IMapper mapper,
            IFileService fileService
        )
        {
            _carRepository = carRepository;
            _assignRepository = assignRepository;
            _userManager = userManager;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ReturnCarDTO>> GetCars(
            string username,
            PaginationRequest paginationRequest
        )
        {
            var manager = await _userManager.FindByNameAsync(username);
            IEnumerable<CarModel> cars;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                cars = await _carRepository.GetManagedCars(
                    manager.Id,
                    paginationRequest.PageNumber,
                    paginationRequest.PageSize
                );
            else
                cars = await _carRepository.GetCars(
                    paginationRequest.PageNumber,
                    paginationRequest.PageSize
                );

            var returnCarTasks = cars.Select(async car =>
            {
                ReturnCarDTO returnCar = _mapper.Map<ReturnCarDTO>(car);
                if (car.CarPictureUrl is not null)
                {
                    returnCar.PictureUrl = await _fileService.RetrieveCarPictureUrl(
                        car.CarPictureUrl
                    );
                }
                return returnCar;
            });

            return await Task.WhenAll(returnCarTasks);
        }

        public async Task<ReturnCarDTO> GetCarById(Guid id, string username)
        {
            var manager = await _userManager.FindByNameAsync(username);
            CarModel? car;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                car = await _carRepository.GetManagedCarById(manager.Id, id);
            else
                car = await _carRepository.GetCarById(id);

            if (car is null)
                throw new CarNotFoundException();

            var returnCar = _mapper.Map<ReturnCarDTO>(car);

            if (car.CarPictureUrl is not null)
                returnCar.PictureUrl = await _fileService.RetrieveCarPictureUrl(car.CarPictureUrl);

            return returnCar;
        }

        public async Task CreateCar(CreateCarDTO createCarDTO, string username)
        {
            if (string.IsNullOrWhiteSpace(createCarDTO.Model))
                throw new EmptyInputException(nameof(createCarDTO.Model));

            CarModel carModel = _mapper.Map<CarModel>(createCarDTO);

            var manager = await _userManager.FindByNameAsync(username);
            await _carRepository.CreateCar(carModel);

            if (createCarDTO.CarPicture is not null)
            {
                carModel.CarPictureUrl = await _fileService.UploadCarPicture(
                    createCarDTO.CarPicture,
                    carModel.Id
                );
                await _carRepository.UpdateCar(carModel);
            }

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                await _assignRepository.AssignCarToHandler(carModel, manager);
        }

        public async Task DeleteCar(Guid id, string username)
        {
            var manager = await _userManager.FindByNameAsync(username);
            CarModel? car;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                car = await _carRepository.GetManagedCarById(manager.Id, id);
            else
                car = await _carRepository.GetCarById(id);

            if (car is null)
                throw new CarNotFoundException();

            if (car.CarPictureUrl is not null)
                await _fileService.DeleteCarPicture(car.CarPictureUrl);

            await _carRepository.DeleteCar(car);
        }

        public async Task UpdateCar(Guid id, UpdateCarDTO updateCarDTO, string username)
        {
            var manager = await _userManager.FindByNameAsync(username);
            CarModel? car = null;

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
                car = await _carRepository.GetManagedCarById(manager.Id, id);
            else
                car = await _carRepository.GetCarById(id);

            if (car is null)
                throw new CarNotFoundException();

            _mapper.Map(updateCarDTO, car);

            await _carRepository.UpdateCar(car);
        }

        public async Task CreateCarBrand(string brand)
        {
            CarBrandModel carBrandModel = new CarBrandModel() { Brand = brand };
            await _carRepository.CreateCarBrand(carBrandModel);
        }

        public async Task CreateCarEngineType(string engineType)
        {
            CarEngineModel carEngineModel = new CarEngineModel() { EngineType = engineType };
            await _carRepository.CreateCarEngineType(carEngineModel);
        }

        public async Task CreateCarBodyType(string bodyType)
        {
            CarBodyModel carBodyModel = new CarBodyModel() { BodyType = bodyType };
            await _carRepository.CreateCarBodyType(carBodyModel);
        }

        public async Task<IEnumerable<CarBodyRequest>> GetCarBodyTypes()
        {
            return _mapper.Map<IEnumerable<CarBodyRequest>>(
                await _carRepository.GetCarBodyModels()
            );
        }

        public async Task<IEnumerable<CarBrandRequest>> GetCarBrands()
        {
            return _mapper.Map<IEnumerable<CarBrandRequest>>(
                await _carRepository.GetCarBrandModels()
            );
        }

        public async Task<IEnumerable<CarEngineRequest>> GetCarEngineTypes()
        {
            return _mapper.Map<IEnumerable<CarEngineRequest>>(
                await _carRepository.GetCarEngineModels()
            );
        }
    }
}
