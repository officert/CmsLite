using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);
        User GetUserByEmail(string email);

        void SignIn(string userName, bool createPersistantCookie);
        void SignOut();

        void ActivateUser(User user);
        User CreateUser(string email, string password);
        bool ChangeUserPassword(string userName, string oldPassword, string newPassword);
        User.UserStatus ValidateUser(User user, string password);

        void SendUserActivationEmail(User user);

        User GetCurrentLoggedInUser();
    }
}
