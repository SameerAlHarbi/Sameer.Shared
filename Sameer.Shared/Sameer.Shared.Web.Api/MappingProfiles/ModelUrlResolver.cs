using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sameer.Shared.Web.Api
{
    public class ModelUrlResolver<M, VM> : IValueResolver<M, VM, string> where M : class, ISameerObject
        , new() where VM : class, IApiViewModel, new()
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModelUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(M source, VM destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[SameerBaseApiController.URLHELPER];
            var controllerName = _httpContextAccessor.HttpContext.Items[SameerBaseApiController.CONTROLLER_NAME];
            string result = url.Link($"{controllerName}_ById", new { id = source.Id });
            return result;
        }
    }
}
