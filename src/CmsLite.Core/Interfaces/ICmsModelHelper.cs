using System.Web;

namespace CmsLite.Core.Interfaces
{
    public interface ICmsModelHelper
    {
        T GetModel<T>(HttpContextBase httpContextBase) where T : class, new();
    }
}
