using AutoMapper;
using Core.Models;
using Core.Requests;
using WebApi.Contracts;

namespace WebApi.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ProfileModel, ReturnProfileDTO>();
            CreateMap<RegisterProfileDTO, ProfileModel>();
            CreateMap<CarModel, ReturnCarDTO>()
                .ForMember(dest => dest.BodyType, act => act.MapFrom(src => src.BodyType.BodyType))
                .ForMember(dest => dest.Brand, act => act.MapFrom(src => src.Brand.Brand))
                .ForMember(
                    dest => dest.EngineType,
                    act => act.MapFrom(src => src.EngineType.EngineType)
                );
            CreateMap<CreateCarDTO, CarModel>()
                .ForMember(dest => dest.BodyType, act => act.Ignore())
                .ForMember(dest => dest.Brand, act => act.Ignore())
                .ForMember(dest => dest.EngineType, act => act.Ignore());
            CreateMap<CarBodyModel, CarBodyRequest>();
            CreateMap<CarBrandModel, CarBrandRequest>();
            CreateMap<CarEngineModel, CarEngineRequest>();
        }
    }
}
