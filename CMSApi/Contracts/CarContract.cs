using Core.Models;
using Core.Requests;

namespace CMSApi.Contracts
{
    public class BaseCar
    {
        public string Model { get; set; }
        public DateTime ManufactureDate { get; set; }
    }

    public class CreateCarDTO : BaseCar
    {
        public CarBrandModel Brand { get; set; }
        public CarBodyModel BodyType { get; set; }
        public CarEngineModel EngineType { get; set; }
        public IFormFile CarPicture { get; set; }
    }

    public class ReturnCarDTO : BaseCar
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string BodyType { get; set; }
        public string EngineType { get; set; }
        public Uri PictureUrl { get; set; }
    }

    public class UpdateCarDTO : BaseCar { }
}
