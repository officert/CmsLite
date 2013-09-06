using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;

namespace CmsLite.Interfaces.Authentication
{
    public interface ICmsLiteHttpContext : IDisposable
    {
        Uri RequestUrl { get; }
        string RedirectLocation { get; set; }
        HttpCookieCollection RequestCookies { get; }
        HttpCookieCollection ResponseCookies { get; }
        IPrincipal CurrentUser { get; }
        NameValueCollection RequestQueryString { get; }
        void ResponseRedirect(Uri uri);
        string RequestApplicationPath { get; }
    }
}
