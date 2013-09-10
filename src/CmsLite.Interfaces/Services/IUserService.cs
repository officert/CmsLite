using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IUserService : IServiceBase<User>
    {
        User Create(string email, string password);

        void ActivateUser(User user);

        bool ChangeUserPassword(string userName, string newPassword);
    }
}
