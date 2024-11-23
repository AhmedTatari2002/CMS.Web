using SMS.Core.Dtos;
using SMS.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Infrastructure.Services.Advertisements

{
    public interface IAdvertisementService
    {
        Task<ResponseDto> GetAll(Pagination pagination, Query query);
        Task<int> Delete(int id);
        Task<int> Create(CreateAdvertisementDto dto);
        Task<List<UserViewModel>> GetAdvertisementOwners();

        Task<UpdateAdvertisementDto> Get(int id);
        Task<int> Update(UpdateAdvertisementDto dto);

    }
}
