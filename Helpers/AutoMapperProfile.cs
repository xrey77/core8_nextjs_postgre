using AutoMapper;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Models.dto;

namespace core8_nextjs_postgre.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<UserRegister, User>();
            CreateMap<UserLogin, User>();
            CreateMap<UserUpdate, User>();
            CreateMap<UserPasswordUpdate, User>();
            CreateMap<Product, ProductModel>();
            CreateMap<ProductModel, Product>();
        }
    }
    

}