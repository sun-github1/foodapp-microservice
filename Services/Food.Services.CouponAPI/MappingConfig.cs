using AutoMapper;
using Food.Services.CouponAPI.Dtos;
using Food.Services.CouponAPI.Models;

namespace Food.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
