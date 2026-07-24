using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class CarModel : BaseModel
    {
        public string Model { get; set; }
        public DateTime ManufactureDate { get; set; }

        [ForeignKey("BrandId")]
        public CarBrandModel Brand { get; set; }
        public Guid BrandId { get; set; }
        [ForeignKey("BodyTypeId")]
        public CarBodyModel BodyType { get; set; }
        public Guid BodyTypeId { get; set; }
        [ForeignKey("EngineTypeId")]
        public CarEngineModel EngineType { get; set; }
        public Guid EngineTypeId { get; set; }
        public string? CarPictureUrl { get; set; }
    }
}
