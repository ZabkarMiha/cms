using AutoMapper;
using WebApi.Repository;
using Core.Models;
using WebApi.Contracts;
using Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using WebApi.Helpers;
using Core.Services;
using Core.Requests;

namespace WebApi.Business
{
    public interface ICarBusinessLayer
    {
        Task<IEnumerable<ReturnCarDTO>> GetUserOwnedCars(
            string Username,
            PaginationRequest paginationRequest
        );
        Task CreateCar(string username, CreateCarDTO createCarDTO);
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

        public async Task<IEnumerable<ReturnCarDTO>> GetUserOwnedCars(
            string Username,
            PaginationRequest paginationRequest
        )
        {
            var user = await _userManager.FindByNameAsync(Username);

            var cars = await _carRepository.GetUserOwnedCars(
                user.Id,
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

        public async Task CreateCar(string username, CreateCarDTO createCarDTO)
        {
            if (string.IsNullOrWhiteSpace(createCarDTO.Model))
                throw new EmptyInputException(nameof(createCarDTO.Model));

            CarModel carModel = _mapper.Map<CarModel>(createCarDTO);

            var user = await _userManager.FindByNameAsync(username);

            await _carRepository.CreateCar(carModel);

            if (createCarDTO.CarPicture is not null)
            {
                carModel.CarPictureUrl = await _fileService.UploadCarPicture(
                    createCarDTO.CarPicture,
                    carModel.Id
                );
                await _carRepository.UpdateCar(carModel);
            }

            await _assignRepository.AssignCarToUser(carModel, user);
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
