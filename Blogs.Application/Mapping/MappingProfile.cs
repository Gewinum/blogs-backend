using AutoMapper;

namespace Blogs.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Models.Blog, Domain.Dtos.BlogDto>();
    }
}