using System;
using System.Web.Helpers;
using CmsLite.Domains.Entities;

namespace CmsLite.Services.Helpers
{
    public static class AuthenticationHelper
    {
        public static string GenerateSalt()
        {
            return Crypto.GenerateSalt();
        }

        public static string CreatePasswordHash(string password, string salt)
        {
            return Crypto.HashPassword(password + salt);
        }

        public static bool VerifyHashedPassword(User user, string password)
        {
            if (user == null)
                throw new ArgumentException("User cannot be null");

            var hashedPassword = user.Password;
            return Crypto.VerifyHashedPassword(hashedPassword, password + user.PasswordSalt);
        }
    }
}