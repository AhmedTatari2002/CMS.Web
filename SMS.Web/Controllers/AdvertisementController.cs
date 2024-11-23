using SMS.Core.Constants;
using SMS.Core.Dtos;
using SMS.Infrastructure.Services.Advertisements;
using SMS.Infrastructure.Services.Categories;
using SMS.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SMS.Web.Controllers
{
    public class AdvertisementController : Controller
    {

        private readonly IAdvertisementService _advertisementService;
        private readonly ILogger<AdvertisementController> _logger;

        public AdvertisementController(ILogger<AdvertisementController> logger,IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetAdvertisementData(Pagination pagination,Query query)
        {
            var result = await _advertisementService.GetAll(pagination, query);
            return  Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["owners"] = new SelectList(await _advertisementService.GetAdvertisementOwners(), "Id", "FullName");
            return View();
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}


        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateAdvertisementDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.OwnerId))
            {
                //_logger.LogInformation("OwnerId provided: {OwnerId}", dto.OwnerId);
                ModelState.Remove("Owner.FullName");
                ModelState.Remove("Owner.Email");
                ModelState.Remove("Owner.PhoneNumber");
                ModelState.Remove("Owner.Image");
            }

            if (ModelState.IsValid)
            {
                await _advertisementService.Create(dto);
                //_logger.LogInformation("تم الوصول إلى Controller: CreateAdvertisement");
                return Ok(Core.Constants.Results.AddSuccessResult());
              

            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
            {
                _logger.LogError("خطأ في التحقق: {Error}", error);
            }
            _logger.LogInformation("Received Image: {Image}", dto.Image?.FileName ?? "No Image Uploaded");
            _logger.LogInformation("Title: {Title}", dto.Title);
            _logger.LogInformation("WebsiteUrl: {WebsiteUrl}", dto.WebsiteUrl);
            _logger.LogInformation("StartDate: {StartDate}", dto.StartDate);
            _logger.LogInformation("EndDate: {EndDate}", dto.EndDate);
            _logger.LogInformation("Price: {Price}", dto.Price);
            //_logger.LogInformation("OwnerId: {OwnerId}", dto.OwnerId);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _advertisementService.Get(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm]UpdateAdvertisementDto dto)
        {
            if (ModelState.IsValid)
            {
                await _advertisementService.Update(dto);
                return Ok(Core.Constants.Results.EditSuccessResult());
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _advertisementService.Delete(id);
            return Ok(Core.Constants.Results.DeleteSuccessResult());
        }

    }
}
