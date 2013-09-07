using AutoMapper;
using CmsLite.Core.App_Start;

namespace CmsLite.Core.Cms.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new AdminProfile());
        }
    }
}