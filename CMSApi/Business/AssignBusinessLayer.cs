using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using CMSApi.Repository;

namespace CMSApi.Business
{
    public interface IAssignBusinessLayer
    {
        Task AssignUserToHandler(Guid UserId, Guid HandlerId);
        Task AssignCarToHandler(Guid CarId, Guid HandlerId);
        Task AssignCarToUser(Guid CarId, Guid UserId, string Username);
        Task UnassignCarFromUser(Guid CarId, Guid UserId, string Username);
    }

    public class AssignBusinessLayer : IAssignBusinessLayer
    {
        private readonly IAssignRepository _assignRepository;
        private readonly UserManager<ProfileModel> _userManager;
        private readonly ICarRepository _carRepository;

        public AssignBusinessLayer(
            IAssignRepository assignRepository,
            UserManager<ProfileModel> userManager,
            ICarRepository carRepository
        )
        {
            _assignRepository = assignRepository;
            _userManager = userManager;
            _carRepository = carRepository;
        }

        public async Task AssignUserToHandler(Guid UserId, Guid HandlerId)
        {
            var user =
                await _userManager.FindByIdAsync(UserId.ToString())
                ?? throw new UserNotFoundException();
            var handler =
                await _userManager.FindByIdAsync(HandlerId.ToString())
                ?? throw new UserNotFoundException();

            if (
                !await _userManager.IsInRoleAsync(user, "Viewer")
                || await _userManager.IsInRoleAsync(user, "Handler")
                || await _userManager.IsInRoleAsync(user, "Admin")
            )
                throw new UserNotInRoleException();

            if (!await _userManager.IsInRoleAsync(handler, "Handler"))
                throw new UserNotInRoleException();

            try
            {
                await _assignRepository.AssignUserToHandler(user, handler);
            }
            catch (DbUpdateException e) when (e.InnerException is PostgresException inner)
            {
                throw new KeyAlreadyExistsException(inner.Detail);
            }
        }

        public async Task AssignCarToHandler(Guid CarId, Guid HandlerId)
        {
            var car = await _carRepository.GetCarById(CarId) ?? throw new CarNotFoundException();
            var handler =
                await _userManager.FindByIdAsync(HandlerId.ToString())
                ?? throw new UserNotFoundException();

            if (!await _userManager.IsInRoleAsync(handler, "Handler"))
                throw new UserNotInRoleException();

            try
            {
                await _assignRepository.AssignCarToHandler(car, handler);
            }
            catch (DbUpdateException e) when (e.InnerException is PostgresException inner)
            {
                throw new KeyAlreadyExistsException(inner.Detail);
            }
        }

        public async Task AssignCarToUser(Guid CarId, Guid UserId, string Username)
        {
            var manager = await _userManager.FindByNameAsync(Username);
            var car = await _carRepository.GetCarById(CarId) ?? throw new CarNotFoundException();
            var user =
                await _userManager.FindByIdAsync(UserId.ToString())
                ?? throw new UserNotFoundException();

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
            {
                if (
                    !(
                        await _assignRepository.HandlerManagesUser(manager.Id, UserId)
                        && await _assignRepository.HandlerManagesCar(manager.Id, CarId)
                    )
                )
                {
                    throw new DoesNotManageException(manager.UserName);
                }
            }

            try
            {
                await _assignRepository.AssignCarToUser(car, user);
            }
            catch (DbUpdateException e) when (e.InnerException is PostgresException inner)
            {
                throw new KeyAlreadyExistsException(inner.Detail);
            }
        }

        public async Task UnassignCarFromUser(Guid CarId, Guid UserId, string Username)
        {
            var manager = await _userManager.FindByNameAsync(Username);
            var car = await _carRepository.GetCarById(CarId) ?? throw new CarNotFoundException();
            var user =
                await _userManager.FindByIdAsync(UserId.ToString())
                ?? throw new UserNotFoundException();

            if (await _userManager.IsInRoleAsync(manager, "Handler"))
            {
                if (
                    !(
                        await _assignRepository.HandlerManagesUser(manager.Id, UserId)
                        && await _assignRepository.HandlerManagesCar(manager.Id, CarId)
                    )
                )
                    throw new DoesNotManageException(manager.UserName);
            }
            else
                throw new UserNotInRoleException();

            try
            {
                await _assignRepository.UnassignCarFromUser(car, user);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UserWithCarDoesntExistException();
            }
        }
    }
}
