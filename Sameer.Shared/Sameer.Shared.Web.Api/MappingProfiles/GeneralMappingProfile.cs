using AutoMapper;

namespace Sameer.Shared.Web.Api
{
    public abstract class GeneralMappingProfile<M, VM> 
        : Profile where M : class, ISameerObject, new() where VM : class, IApiViewModel, new()
    {
        public GeneralMappingProfile()
        {
            CreateMap<M, VM>()
                .ForMember(s => s.Url, opt => opt.MapFrom<ModelUrlResolver<M, VM>>())
                .ReverseMap();
        }
    }
}
