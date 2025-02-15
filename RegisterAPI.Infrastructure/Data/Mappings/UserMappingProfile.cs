using AutoMapper;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Infrastructure.Data.EntityModels;
using RegisterAPI.Model.Core.Model.Dto;

namespace RegisterAPI.Infrastructure.Data.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<UserEntityModel, UserDto>(); // Entity to DTO
            CreateMap<UserDto, UserEntityModel>(); // DTO to Entity
            CreateMap<TransactionDto, TransactionEntityModel>();
            CreateMap<TransactionEntityModel, TransactionDto>();
        }
    }
}
