using System.Web.Security;
using CmsLite.Interfaces.Authentication;

namespace CmsLite.Core.Authetication
{
    public class SimpleAuthenticationProvider : IAuthentication
    {
        private readonly ICmsLiteHttpContext _httpContext;

        public SimpleAuthenticationProvider(ICmsLiteHttpContext context)
        {
            _httpContext = context;
        }

        public void SignIn(string userName, bool createPersistantCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistantCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public string CurrentUserName()
        {
            return _httpContext.CurrentUser.Identity.Name;
        }
    }
}
