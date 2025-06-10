using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class HomeController : BaseWithUserApiController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IMastersService ms) : base(userManager, signInManager, mapper, ms)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("API - Server");
        }

    }
}
