using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMSApi.Business;
using CMSApi.Contracts;
using Core.Requests;
using Core.Models;

namespace CMSApi.Controllers
{
    [Authorize(Roles = "Admin, Handler")]
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
        public async Task<IEnumerable<ReturnCarDTO>> GetCarsAsync(
            [FromQuery] PaginationRequest paginationRequest
        )
        {
            return await _carBusinessLayer.GetCars(User.Identity.Name, paginationRequest);
        }

        [Route("GetCarById/{id:Guid}")]
        [HttpGet]
        public async Task<ReturnCarDTO> GetCarByIdAsync(Guid id)
        {
            return await _carBusinessLayer.GetCarById(id, User.Identity.Name);
        }

        [Route("CreateCar")]
        [HttpPost]
        public async Task<IActionResult> CreateCarAsync([FromForm] CreateCarDTO createCarDTO)
        {
            await _carBusinessLayer.CreateCar(createCarDTO, User.Identity.Name);
            return Ok();
        }

        [Route("DeleteCar/{id:Guid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCarAsync(Guid id)
        {
            await _carBusinessLayer.DeleteCar(id, User.Identity.Name);
            return Ok();
        }

        [Route("UpdateCar/{id:Guid}")]
        [HttpPut]
        public async Task<IActionResult> UpdateCarAsync(
            Guid id,
            [FromBody] UpdateCarDTO updateCarDTO
        )
        {
            await _carBusinessLayer.UpdateCar(id, updateCarDTO, User.Identity.Name);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("CreateCarBodyType/{bodyType}")]
        [HttpPost]
        public async Task<IActionResult> CreateCarBodyTypeAsync(string bodyType)
        {
            await _carBusinessLayer.CreateCarBodyType(bodyType);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("CreateCarEngineType/{engineType}")]
        [HttpPost]
        public async Task<IActionResult> CreateCarEngineTypeAsync(string engineType)
        {
            await _carBusinessLayer.CreateCarEngineType(engineType);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("CreateCarBrand/{brand}")]
        [HttpPost]
        public async Task<IActionResult> CreateCarBrandAsync(string brand)
        {
            await _carBusinessLayer.CreateCarBrand(brand);
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
