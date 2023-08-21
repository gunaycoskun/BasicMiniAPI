using AutoMapper;
using MiniAPI.Models;
using MiniAPI.Models.DTO;

namespace MiniAPI.MappingConfigs
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Coupon, Models.DTO.CouponDTO>().ReverseMap();
            CreateMap<Models.DTO.CouponCreateDTO, Models.Coupon>().ReverseMap();
            CreateMap<Models.DTO.CouponUpdateDTO, Models.Coupon>().ReverseMap();
            CreateMap<UserDTO, LocalUser>().ReverseMap();
        }
    }
}
