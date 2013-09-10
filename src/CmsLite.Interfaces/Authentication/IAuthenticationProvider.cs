using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Authentication
{
    public interface IAuthenticationProvider
    {
        void SignIn(string userName, bool createPersistantCookie);
        void SignOut();
        string GetCurrentUserName();
        User.UserStatus VerifyUser(User user, string password);
        void SendUserActivationEmail(User user);
    }
}
