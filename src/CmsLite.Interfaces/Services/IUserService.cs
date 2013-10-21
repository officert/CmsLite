using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IUserService
    {
        User GetByEmail(string email);

        User Create(string email, string password);

        void ActivateUser(User user);

        bool ChangeUserPassword(string userName, string newPassword);
    }
}
