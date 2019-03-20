using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheWorldProject.Models;
using TheWorldProject.ViewModels;
using AutoMapper;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using TheWorldProject.Services;

namespace TheWorldProject.Controllers
{
    public class SignupController : Controller
    {
        private SignInManager<WorldUser> _signInManager;
        private UserManager<WorldUser> _userManager;
        private Services.IMailService _mailService;

        public SignupController(SignInManager<WorldUser> signInManager, UserManager<WorldUser> userManager, Services.IMailService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mailService = mailService;
        }

        public IActionResult EmailConfirmation()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

       
        //[HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(/*[FromForm]*/RegisterViewModel vm, string returnUrl)
        {
            if(vm.Email.Contains("aol.com") || vm.Email.Contains("amail.com"))
            {
                ModelState.AddModelError("", "We don't support this email domain addresses ");
            }
            if (ModelState.IsValid)
            {
                if(await _userManager.FindByEmailAsync(vm.Email) == null) 
                {
                    WorldUser user = Mapper.Map<WorldUser>(vm);

                    var result = await _userManager.CreateAsync(user, vm.Password);
                    if(result.Succeeded)
                    {
                        var codes = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Signup",
                            new { userId = user.Id, code = codes },
                            protocol: HttpContext.Request.Scheme
                       );

                        DebugMailService mailService = new DebugMailService();
                        await mailService.ConfirmationByEmailMsgAsync(vm.Email, "Confirm Your account, please.",
                            $"Confirm registration folowing the link: <a href='{callbackUrl}'>Confirm email NOW</a>");

                        return RedirectToAction("Index", "App");
                    }
                    else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else 
                {
                    ModelState.AddModelError(vm.Email, "This email is already registered!");
                }
            }

            return View(vm);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("index", "App");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return View("Error");
            }

            var result = _userManager.ConfirmEmailAsync(user, code);
            return View("EmailConfirmation");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPassViewModel vm)
        {
            if (vm.Email.Contains("aol.com") || vm.Email.Contains("amail.com"))
            {
                ModelState.AddModelError("", "We don't support this email domain addresses ");
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
            {
                ModelState.AddModelError("Email", "This email does not registered!");
            }

            if (ModelState.IsValid)
            {

                var codes = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(SignupController.ResetPassword), "Signup",
                    new { userId = user.Id, code = codes }, protocol: HttpContext.Request.Scheme);

                await _mailService.ConfirmationByEmailMsgAsync(vm.Email, "Reset Password", 
                    $"Click - <a href='{callbackUrl}'>HERE</a> - to reset your password");
            }

            return View(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string code = null)
        {
            var vm = new ResetPassViewModel { Code = code };
            return View(vm);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm); 
            }

            var user = await _userManager.FindByEmailAsync(vm.Email); 
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Signup");
            }

            var result = await _userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Signup");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();

        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}