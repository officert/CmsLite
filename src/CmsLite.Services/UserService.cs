using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Services.Helpers;

namespace CmsLite.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthentication _authentication;

        public UserService(IUnitOfWork unitOfWork, IAuthentication authentication)
        {
            _unitOfWork = unitOfWork;
            _authentication = authentication;
        }

        #region Get User

        public User GetUserByUserName(string userName)
        {
            return _unitOfWork.Context.GetDbSet<User>().FirstOrDefault(u => u.UserName == userName);
        }

        public User GetUserByEmail(string email)
        {
            return _unitOfWork.Context.GetDbSet<User>().FirstOrDefault(u => u.Email == email);
        }

        #endregion

        #region Authentication

        public void SignIn(string userName, bool createPersistantCookie)
        {
            var user = _unitOfWork.Context.GetDbSet<User>().FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                _unitOfWork.Commit();
                _authentication.SignIn(userName, createPersistantCookie);
            }
        }

        public void SignOut()
        {
            _authentication.SignOut();
        }

        public User.UserStatus ValidateUser(User user, string password)
        {
            var hashedPassword = AuthenticationHelper.CreatePasswordHash(password, user.PasswordSalt);
            var passwordsMatch = user.Password == hashedPassword;
            var userIsActivated = user.IsActivated;
            var userIsLockedOut = user.IsLockedOut;
            if (!passwordsMatch)
                return User.UserStatus.PasswordDoesNotMatch;
            if (!userIsActivated)
                return User.UserStatus.NotActivated;
            if (userIsLockedOut == true)
                return User.UserStatus.LockedOut;
            return User.UserStatus.Validated;
        }

        #endregion

        public void ActivateUser(User user)
        {
            user.IsActivated = true;
            user.LastModifiedDate = DateTime.UtcNow;
            user.NewEmailKey = null;

            _unitOfWork.Commit();
        }

        public User CreateUser(string email, string password)
        {
            if (EmaillAlreadyExists(email))
                throw new ArgumentException("The email is already in user by another user.");   //TODO: move to resource

            var currentDateTime = DateTime.UtcNow;

            var newUser = new User
                              {
                                  UserName = GenerateNewUserName()
                              };


            if (UserNameAlreadyExists(newUser.UserName))
                throw new ArgumentException("The username is already in user by another user.");   //TODO: move to resource

            newUser.Email = email;
            newUser.PasswordSalt = AuthenticationHelper.GenerateSalt();
            newUser.Password = AuthenticationHelper.CreatePasswordHash(password, newUser.PasswordSalt);
            newUser.JoinDate = currentDateTime;
            newUser.IsActivated = false;
            newUser.IsLockedOut = false;
            newUser.LastLockedOutDate = currentDateTime;
            newUser.LastLoginDate = currentDateTime;
            newUser.LastModifiedDate = currentDateTime;
            newUser.NewEmailKey = Guid.NewGuid().ToString();

            _unitOfWork.Context.GetDbSet<User>().Add(newUser);
            _unitOfWork.Commit();

            return newUser;
        }

        public bool ChangeUserPassword(string userName, string oldPassword, string newPassword)
        {
            var user = GetUserByUserName(userName);
            if (user.Password == AuthenticationHelper.CreatePasswordHash(oldPassword, user.PasswordSalt))
            {
                user.Password = AuthenticationHelper.CreatePasswordHash(newPassword, user.PasswordSalt);

                _unitOfWork.Context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SendUserActivationEmail(User user)
        {
            var smtpClient = new SmtpClient();
            //TODO: use a different SMTP server
            var fromAddress = new MailAddress("timothyofficer@gmail.com", "Tim Officer");
            string fromPassword = "#rabit01#08";
            var toAddress = new MailAddress(user.Email, user.UserName);
            string subject = "Welcome to Ride Together.com!";
            string messageBody = "Welcome to Ride Together! Thank you for signing up. To finish creating your account please click http://localhost:20000/Account/Activate?email=" + user.Email + "&key=" + user.NewEmailKey;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
            smtpClient.Send(fromAddress.Address, toAddress.Address, subject, messageBody);
        }

        public User GetCurrentLoggedInUser()
        {
            var loggedInUserName = _authentication.CurrentUserName();
            return _unitOfWork.Context.GetDbSet<User>().FirstOrDefault(u => u.UserName == loggedInUserName);
        }

        #region Private Helpers

        public bool UserNameAlreadyExists(string username)
        {
            var foundUsers = _unitOfWork.Context.GetDbSet<User>().Any(u => u.UserName == username);
            return foundUsers;
        }

        public bool EmaillAlreadyExists(string email)
        {
            var foundUsers = _unitOfWork.Context.GetDbSet<User>().Any(u => u.Email == email);
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
