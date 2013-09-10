using System;
using System.Net;
using System.Net.Mail;
using System.Web.Helpers;
using System.Web.Security;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Authentication;
using CmsLite.Resources;
using CmsLite.Services.Helpers;

namespace CmsLite.Core.Authetication
{
    public class SimpleAuthenticationProvider : IAuthenticationProvider
    {
        private readonly ICmsLiteHttpContext _httpContext;

        public SimpleAuthenticationProvider(ICmsLiteHttpContext context)
        {
            _httpContext = context;
        }

        public void SignIn(string userName, bool createPersistantCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistantCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public string GetCurrentUserName()
        {
            return _httpContext.CurrentUser.Identity.Name;
        }

        public User.UserStatus VerifyUser(User user, string password)
        {
            if(user == null)
                throw new ArgumentException("User cannot be null");

            var isVerified = AuthenticationHelper.VerifyHashedPassword(user, password);

            if (!isVerified)
                return User.UserStatus.PasswordDoesNotMatch;

            if (!user.IsActivated)
                return User.UserStatus.NotActivated;

            if (user.IsLockedOut)
                return User.UserStatus.LockedOut;

            return User.UserStatus.Validated;
        }

        public void SendUserActivationEmail(User user)
        {
            throw new NotImplementedException();
            //var smtpClient = new SmtpClient();
            ////TODO: use a different SMTP server
            //var fromAddress = new MailAddress("timothyofficer@gmail.com", "Tim Officer");
            //string fromPassword = "#rabit01#08";
            //var toAddress = new MailAddress(user.Email, user.UserName);
            //string subject = "Welcome to Ride Together.com!";
            //string messageBody = "Welcome to Ride Together! Thank you for signing up. To finish creating your account please click http://localhost:20000/Account/Activate?email=" + user.Email + "&key=" + user.NewEmailKey;
            //smtpClient.Host = "smtp.gmail.com";
            //smtpClient.Port = 587;
            //smtpClient.EnableSsl = true;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtpClient.UseDefaultCredentials = false;
            //smtpClient.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
            //smtpClient.Send(fromAddress.Address, toAddress.Address, subject, messageBody);
        }
    }
}
