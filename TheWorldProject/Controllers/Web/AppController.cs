﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;
using TheWorldProject.Models;
using TheWorldProject.Services;
using TheWorldProject.ViewModels;

namespace TheWorldProject.Controllers.Web
{
    public class AppController: Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository repository,
            ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Contact()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel Model)
        {
            if (Model.Email.Contains("aol.com"))
            {
                ModelState.AddModelError("", "We don't support AOL addresses ");
            }
            if (ModelState.IsValid)
            {
                //_mailService.SendMail(_config["MailSettings:ToAddress"], Model.Email, "From TheWorld Project", Model.Message);
                _mailService.SendMessage(Model.Name, Model.Email, Model.Message);

                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize] 
        public IActionResult Trips()
        {
            return View(); 
        }
    }
}
