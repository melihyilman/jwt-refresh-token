﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userApp.Models;
using userApp.Models.interfaces;
using userApp.Repository;

namespace userApp.Controllers
{
  
    public class UserController : Controller
    {
        private readonly UserRepository userRepository;
        public UserController(IConfiguration configuration)
        {
          
               userRepository = new UserRepository(configuration);


        }
        // GET: UserController
        [userApp.Controllers.Utils.Authorize]
        public ActionResult Index()
        {
            return View(userRepository.FindAll());
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                userRepository.Add(user);
                return RedirectToAction("Index");
            }
            return View(user);
        }



        // GET: UserController/Edit/5
        //[Authorize(Policy = "User")]
        [userApp.Controllers.Utils.Authorize]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            User obj = userRepository.FindByID(id.Value);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [userApp.Controllers.Utils.Authorize]
        public ActionResult Edit(User obj)
        {
            if (ModelState.IsValid)
            {
                userRepository.Update(obj);
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            userRepository.Remove(id.Value);
            return RedirectToAction("Index");
        }

      
    }
}
