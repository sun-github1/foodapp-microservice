using AutoMapper;

namespace Food.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<ProductDto, Product>().ReverseMap();
                //config.CreateMap<CartDto, Cart>().ReverseMap();
                //config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
