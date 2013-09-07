using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Attributes;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    public class AccountController : AdminBaseController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [ImportModelStateFromTempData]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost, ExportModelStateToTempData]
        public ActionResult SignIn(SignInModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(viewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("UserNotFound", Messages.UserNameNotFound);
                    return RedirectToAction("SignIn");
                }
                var userStatus = _userService.ValidateUser(user, viewModel.Password);

                if (userStatus == Domains.Entities.User.UserStatus.PasswordDoesNotMatch)
                {
                    ModelState.AddModelError("PasswordDoesNotMatch", Messages.PasswordDoesNotMatch);
                    return RedirectToAction("SignIn");
                }
                if (userStatus == Domains.Entities.User.UserStatus.NotActivated)
                    return View("NotActivated", new NotActivatedModel { Email = viewModel.Email });

                //if (userStatus == Domains.Entities.User.UserStatus.LockedOut)
                //{
                //    ModelState.AddModelError("LockedOut", Authentication_Messages.LogOn_User_Is_Locked_Out);        //TODO: handle this by allowing user to retrieve their account somehow
                //    return View(viewModel);
                //}
                if (userStatus == Domains.Entities.User.UserStatus.Validated)
                {
                    var userDataValues = new List<string>() { user.Email, user.FirstName, user.LastName };

                    _userService.SignIn(user.UserName, viewModel.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
            }
            return RedirectToAction("SignIn");
        }

        public ActionResult SignOut()
        {
            _userService.SignOut();
            return RedirectToAction("SignIn");
        }

        public ActionResult Register()
        {
            var viewModel = new RegisterModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var emailAddress = new MailAddress(viewModel.Email);

                var user = _userService.CreateUser(viewModel.Email, viewModel.Password);    //throws unhandled data exception, to be caught by 404 handler
                _userService.SendUserActivationEmail(user);
                return RedirectToAction("NotActivated", "Account", new { email = user.Email });
            }
            return RedirectToAction("Register");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool changePasswordSucceeded;
        //        try
        //        {
        //            changePasswordSucceeded = _userService.ChangeUserPassword(User.Identity.DisplayName, model.OldPassword, model.NewPassword);
        //        }
        //        catch (Exception)
        //        {
        //            changePasswordSucceeded = false;
        //        }

        //        if (changePasswordSucceeded)
        //        {
        //            return RedirectToAction("ChangePasswordSuccess");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //public ActionResult ChangePasswordSuccess()
        //{
        //    return View();
        //}

        public ActionResult Activate(string email, string key = "")
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
            {
                //TODO: redirect / add modelState error
            }
            if (string.IsNullOrEmpty(key) && !user.IsActivated)
            {
                return RedirectToAction("NotActivated", new { email = user.Email });
            }
            if (user.NewEmailKey == key)
            {
                _userService.ActivateUser(user);
                return RedirectToAction("Activated");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //public ActionResult ReSendActivationEmail(string email)
        //{
        //    try
        //    {
        //        var user = _userService.GetUserByEmail(email);
        //        _userService.SendUserActivationEmail(user);
        //        return View("NotActivated", new NotActivatedModel { Email = user.Email });
        //    }
        //    catch (Exception)
        //    {
        //        //TODO: handle exception
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //[MvcSiteMapNode(Title = "Not Activated", ParentKey = "Account", Key = "Not Activated", Clickable = false)]
        //public ActionResult NotActivated(string email)
        //{
        //    var viewModel = new NotActivatedModel()
        //    {
        //        Email = email
        //    };
        //    return View(viewModel);
        //}

        public ActionResult Activated()
        {
            return View();
        }
    }
}
