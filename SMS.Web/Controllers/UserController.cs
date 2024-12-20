﻿using SMS.Core.Constants;
using SMS.Core.Dtos;
using SMS.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Web.Controllers
{

    public class UserController : Controller
    {

        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }



        //[HttpGet]
        //public async Task<IActionResult> ExportToExcel()
        //{
        //   return File(await _userService.ExportToExcel(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        //}



        //[HttpGet]
        //public async Task<IActionResult> AddFCM(string fcm)
        //{
        //    await _userService.SetFCMToUser(userId, fcm);
        //    return Ok("Updated FCM User");
        //}

       
        public async Task<JsonResult> GetUserData(Pagination pagination,Query query)
        {
            var result = await _userService.GetAll(pagination, query);
            return  Json(result);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            if (ModelState.IsValid)
            {
                await _userService.Create(dto);
                return Ok(SMS.Core.Constants.Results.AddSuccessResult());
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userService.Get(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] UpdateUserDto dto)
        {
            if (ModelState.IsValid)
            {
                await _userService.Update(dto);
                return Ok(SMS.Core.Constants.Results.EditSuccessResult());
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.Delete(id);
            return Ok(SMS.Core.Constants.Results.DeleteSuccessResult());
        }

    }
}
