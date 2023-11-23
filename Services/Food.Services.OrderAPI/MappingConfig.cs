using AutoMapper;
using Food.Services.OrderAPI.Messages;
using Food.Services.OrderAPI.Models;

namespace Food.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CheckoutHeaderDto, OrderHeader>().ReverseMap();
                config.CreateMap<CartDetailsDto, OrderDetails>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
