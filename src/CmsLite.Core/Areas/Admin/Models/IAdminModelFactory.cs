namespace CmsLite.Core.Areas.Admin.Models
{
    public interface IAdminModelFactory
    {
        T Create<T>() where T : AdminContextModel, new();
        void Initialize<T>(T model) where T : AdminContextModel, new();
    }
}