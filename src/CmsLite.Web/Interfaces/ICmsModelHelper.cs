using System.Web;

namespace CmsLite.Web.Interfaces
{
    public interface ICmsModelHelper
    {
        T GetModel<T>(HttpContextBase httpContextBase) where T : class, new();
    }
}
