using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using CmsLite.Interfaces.Authentication;

namespace CmsLite.Core.Authetication
{
    public class CmsLiteHttpContext : ICmsLiteHttpContext
    {
        public HttpContext Current
        {
            get { return HttpContext.Current; }
        }

        public Uri RequestUrl
        {
            get { return Current.Request.Url; }
        }

        public string RedirectLocation
        {
            get { return Current.Response.RedirectLocation; }
            set { Current.Response.RedirectLocation = value; }
        }

        public HttpCookieCollection RequestCookies
        {
            get { return Current.Request.Cookies; }
        }

        public HttpCookieCollection ResponseCookies
        {
            get { return Current.Response.Cookies; }
        }

        public IPrincipal CurrentUser
        {
            get { return Current.User; }
        }

        public NameValueCollection RequestQueryString
        {
            get { return Current.Request.QueryString; }
        }

        public void ResponseRedirect(Uri uri)
        {
            Current.Response.Redirect(uri.AbsoluteUri);
        }

        public string RequestApplicationPath
        {
            get { return Current.Request.ApplicationPath; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
