using System;
using System.Security.Cryptography;
using System.Web.Security;

namespace CmsLite.Services.Helpers
{
    public static class AuthenticationHelper
    {
        public static string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            var saltAndPwd = String.Concat(pwd, salt);
            var hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
            return hashedPwd;
        }
    }
}