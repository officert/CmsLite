using AutoMapper;
using CmsLite.Web.App_Start;

namespace CmsLite.Web.Cms.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new AdminProfile());
        }
    }
}