using AutoMapper;
using CmsLite.Core.Automapper;

namespace CmsLite.Core.App_Start
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new AdminProfile());
        }
    }
}