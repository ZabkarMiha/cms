using Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Business;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin, Handler, Viewer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarBusinessLayer _carBusinessLayer;

        public CarController(ICarBusinessLayer carBusinessLayer)
        {
            _carBusinessLayer = carBusinessLayer;
        }

        [Route("GetCars")]
        [HttpGet]
        public async Task<IEnumerable<ReturnCarDTO>> GetUserOwnedCarsAsync(
            [FromQuery] PaginationRequest paginationRequest
        )
        {
            return await _carBusinessLayer.GetUserOwnedCars(User.Identity.Name, paginationRequest);
        }

        [Route("CreateCar")]
        [HttpPost]
        public async Task<IActionResult> CreateCarAsync([FromForm] CreateCarDTO createCarDTO)
        {
            await _carBusinessLayer.CreateCar(User.Identity.Name, createCarDTO);
            return Ok();
        }

        [Route("GetCarBodyTypes")]
        [HttpGet]
        public async Task<IEnumerable<CarBodyRequest>> GetCarBodyTypesAsync(){
            return await _carBusinessLayer.GetCarBodyTypes();
        }

        [Route("GetCarBrands")]
        [HttpGet]
        public async Task<IEnumerable<CarBrandRequest>> GetCarBrandsAsync(){
            return await _carBusinessLayer.GetCarBrands();
        }

        [Route("GetCarEngineTypes")]
        [HttpGet]
        public async Task<IEnumerable<CarEngineRequest>> GetCarEngineTypesAsync(){
            return await _carBusinessLayer.GetCarEngineTypes();
        }
    }
}
