using AutoMapper;
using SMS.Core.Dtos;
using SMS.Core.Enums;
using SMS.Core.Exceptions;
using SMS.Core.ViewModels;
using SMS.Data;
using SMS.Data.Models;
using SMS.Infrastructure.Services.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Core.Constants;
using Microsoft.Extensions.Logging;

namespace SMS.Infrastructure.Services.Advertisements
{
    public class AdvertisementService : IAdvertisementService
    {

        private readonly CMSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IUserService  _userService;
        private readonly IFileService _fileService;
        private readonly ILogger<AdvertisementService> _logger;

        public AdvertisementService(ILogger<AdvertisementService> logger,IFileService fileService,CMSDbContext db, IMapper mapper, IUserService userService)
        {
            _db = db;
            _mapper = mapper;
            _fileService = fileService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<List<UserViewModel>> GetAdvertisementOwners()
        {
            var users = await _db.Users.Where(x => !x.IsDelete && x.UserType == UserType.AdvertisementOwner).ToListAsync();
            return _mapper.Map<List<UserViewModel>>(users);
        }

        public async Task<ResponseDto> GetAll(Pagination pagination, Query query)
        {
            var queryString = _db.Advertisements.Include(x => x.Owner).Where(x => !x.IsDelete).AsQueryable();

            var dataCount = queryString.Count();
            var skipValue = pagination.GetSkipValue();
            var dataList = await queryString.Skip(skipValue).Take(pagination.PerPage).ToListAsync();
            var advertisements = _mapper.Map<List<AdvertisementViewModel>>(dataList);
            var pages = pagination.GetPages(dataCount);
            var result = new ResponseDto
            {
                data = advertisements,
                meta = new Meta
                {
                    page = pagination.Page,
                    perpage = pagination.PerPage,
                    pages = pages,
                    total = dataCount,
                }
            };
            return result;
        }

        public async Task<int> Delete(int id)
        {
            var advertisement = await _db.Advertisements.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if(advertisement == null)
            {
                throw new EntityNotFoundException();
            }
            advertisement.IsDelete = true;
            _db.Advertisements.Update(advertisement);
            await _db.SaveChangesAsync();
            return advertisement.Id;
        }

        public async Task<UpdateAdvertisementDto> Get(int id)
        {
            var advertisement = await _db.Advertisements.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (advertisement == null)
            {
                throw new EntityNotFoundException();
            }
            return _mapper.Map<UpdateAdvertisementDto>(advertisement);
        }


        public async Task<int> Create(CreateAdvertisementDto dto)
        {

            if (dto.StartDate >= dto.EndDate)
            {
                throw new InvalidDateException();
            }

            var advertisement = _mapper.Map<Advertisement>(dto);
        
            if (dto.Image != null)
            {
                advertisement.ImageUrl = await _fileService.SaveFile(dto.Image, FolderNames.ImagesFolder);

            }

            if (!string.IsNullOrWhiteSpace(dto.OwnerId))
            {
                advertisement.OwnerId = dto.OwnerId;

            }

            await _db.Advertisements.AddAsync(advertisement);
            await _db.SaveChangesAsync();

            if (advertisement.OwnerId == null)
            {
                var userId = await _userService.Create(dto.Owner);
                advertisement.OwnerId = userId;

                _db.Advertisements.Update(advertisement);
                await _db.SaveChangesAsync();

            }

            return advertisement.Id;
        }

        //public async Task<int> Create(CreateAdvertisementDto dto)
        //{
        //    try
        //    {
        //        // تسجيل البداية
        //        _logger.LogInformation("بدأ تنفيذ عملية إنشاء الإعلان.");

        //        // التحقق من التاريخ
        //        if (dto.StartDate >= dto.EndDate)
        //        {
        //            _logger.LogError("InvalidDateException: تاريخ البداية أكبر من أو يساوي تاريخ النهاية.");
        //            throw new InvalidDateException();
        //        }

        //        // تحويل الكائن باستخدام _mapper
        //        var advertisement = _mapper.Map<Advertisement>(dto);
        //        _logger.LogInformation("تم تحويل الكائن بنجاح باستخدام _mapper.");

        //        // حفظ الصورة إن وجدت
        //        if (dto.Image != null)
        //        {
        //            _logger.LogInformation("بدأت عملية حفظ الصورة.");
        //            advertisement.ImageUrl = await _fileService.SaveFile(dto.Image, FolderNames.ImagesFolder);
        //            _logger.LogInformation("تم حفظ الصورة بنجاح. المسار: {ImageUrl}", advertisement.ImageUrl);
        //        }

        //        // التحقق من OwnerId
        //        if (!string.IsNullOrWhiteSpace(dto.OwnerId))
        //        {
        //            _logger.LogInformation("تم العثور على OwnerId وتم تعيينه مباشرة.");
        //            advertisement.OwnerId = dto.OwnerId;
        //        }

        //        // إضافة الإعلان إلى قاعدة البيانات
        //        await _db.Advertisements.AddAsync(advertisement);
        //        _logger.LogInformation("تمت إضافة الإعلان إلى قائمة السياق بنجاح.");
        //        await _db.SaveChangesAsync();
        //        _logger.LogInformation("تم حفظ الإعلان في قاعدة البيانات.");

        //        // إنشاء مستخدم جديد إذا لم يكن OwnerId موجودًا
        //        if (advertisement.OwnerId == null)
        //        {
        //            _logger.LogInformation("لم يتم العثور على OwnerId. سيتم إنشاء مالك جديد.");
        //            var userId = await _userService.Create(dto.Owner);
        //            advertisement.OwnerId = userId;
        //            _logger.LogInformation("تم إنشاء المستخدم الجديد بنجاح. OwnerId: {OwnerId}", userId);

        //            // تحديث الإعلان
        //            _db.Advertisements.Update(advertisement);
        //            await _db.SaveChangesAsync();
        //            _logger.LogInformation("تم تحديث الإعلان بالمالك الجديد.");
        //        }

        //        // تسجيل النهاية بنجاح
        //        _logger.LogInformation("تم إنشاء الإعلان بنجاح. AdvertisementId: {Id}", advertisement.Id);

        //        return advertisement.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        // تسجيل الخطأ
        //        _logger.LogError(ex, "حدث خطأ أثناء إنشاء الإعلان.");
        //        throw; // إعادة رمي الاستثناء
        //    }
        //}


        public async Task<int> Update(UpdateAdvertisementDto dto)
        {

            if (dto.StartDate >= dto.EndDate)
            {
                throw new InvalidDateException();
            }

            var advertisement = await _db.Advertisements.SingleOrDefaultAsync(x => x.Id == dto.Id && !x.IsDelete);
            if (advertisement == null)
            {
                throw new EntityNotFoundException();
            }

            var updatedAdvertisement = _mapper.Map(dto, advertisement);

            //if (dto.Image != null)
            //{
            //    updatedAdvertisement.ImageUrl = await _fileService.SaveFile(dto.Image, "Images");
            //}

            _db.Advertisements.Update(updatedAdvertisement);
            await _db.SaveChangesAsync();

            return advertisement.Id;
        }

    }
}
