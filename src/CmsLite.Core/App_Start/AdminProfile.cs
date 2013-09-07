using AutoMapper;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Domains.Entities;

namespace CmsLite.Core.App_Start
{
    public class AdminProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<PageNode, PageViewModel>()
                .ForMember(desc => desc.PageTemplates, opts => opts.MapFrom(src => src.PageTemplate.PageTemplates))
                .ForMember(desc => desc.IconImageName, opts => opts.MapFrom(src => src.PageTemplate.IconImageName));
            CreateMap<SectionNode, SectionViewModel>()
                .ForMember(desc => desc.PageTemplates, opts => opts.MapFrom(src => src.SectionTemplate.PageTemplates))
                .ForMember(desc => desc.IconImageName, opts => opts.MapFrom(src => src.SectionTemplate.IconImageName));
            CreateMap<Property, PropertyViewModel>()
                .ForMember(desc => desc.DisplayName, opts => opts.MapFrom(src => src.PropertyTemplate.DisplayName))
                .ForMember(desc => desc.CmsPropertyName, opts => opts.MapFrom(src => src.PropertyTemplate.CmsPropertyType))
                .ForMember(desc => desc.Description, opts => opts.MapFrom(src => src.PropertyTemplate.Description))
                .ForMember(desc => desc.Required, opts => opts.MapFrom(src => src.PropertyTemplate.Required))
                .ForMember(desc => desc.TabName, opts => opts.MapFrom(src => src.PropertyTemplate.TabName));
            CreateMap<PropertyViewModel, Property>();
            CreateMap<SectionTemplate, SectionTemplateViewModel>();
            CreateMap<PageTemplate, PageTemplateViewModel>();
        }
    }
}