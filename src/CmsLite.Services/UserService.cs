using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services.Helpers;

namespace CmsLite.Services
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void ActivateUser(User user)
        {
            user.IsActivated = true;
            user.LastModifiedDate = DateTime.UtcNow;
            user.NewEmailKey = null;

            _unitOfWork.Commit();
        }

        public User Create(string email, string password)
        {
            var userDbSet = _unitOfWork.Context.GetDbSet<User>();

            if (userDbSet.Any(x => x.Email.ToLower() == email.ToLower()))
                throw new ArgumentException(string.Format(Messages.EmailAlreadyExists, email));

            var newUser = new User
                              {
                                  UserName = GenerateNewUserName()
                              };

            if (UserNameAlreadyExists(newUser.UserName))
                throw new ArgumentException("The username is already in user by another user.");   //TODO: move to resource

            newUser.Email = email;
            newUser.PasswordSalt = AuthenticationHelper.GenerateSalt();
            newUser.Password = AuthenticationHelper.CreatePasswordHash(password, newUser.PasswordSalt);
            newUser.CreateDate = DateTime.UtcNow;
            newUser.IsActivated = false;
            newUser.IsLockedOut = false;
            newUser.LastLockedOutDate = newUser.CreateDate;
            newUser.LastLoginDate = newUser.CreateDate;
            newUser.LastModifiedDate = newUser.CreateDate;
            newUser.NewEmailKey = Guid.NewGuid().ToString();

            _unitOfWork.Context.GetDbSet<User>().Add(newUser);
            _unitOfWork.Commit();

            return newUser;
        }

        public bool ChangeUserPassword(string userName, string newPassword)
        {
            var userDbSet = UnitOfWork.Context.GetDbSet<User>();

            var user = userDbSet.FirstOrDefault(x => x.UserName == userName);

            if(user == null)
                throw new ArgumentException(string.Format(Messages.UserNotFound, userName));

            if (AuthenticationHelper.VerifyHashedPassword(user, newPassword))
            {
                user.Password = AuthenticationHelper.CreatePasswordHash(newPassword, user.PasswordSalt);

                _unitOfWork.Context.SaveChanges();

                return true;
            }

            return false;
        }

        #region Private Helpers

        public bool UserNameAlreadyExists(string username)
        {
            var foundUsers = _unitOfWork.Context.GetDbSet<User>().Any(u => u.UserName == username);
            return foundUsers;
        }

        private string GenerateNewUserName()
        {
            var numUsers = _unitOfWork.Context.GetDbSet<User>().Count();
            return "User" + ++numUsers;
        }

        #endregion
    }
}
