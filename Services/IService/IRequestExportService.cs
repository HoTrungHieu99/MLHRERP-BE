using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IRequestExportService
    {
        Task<List<RequestExportDto>> GetAllRequestExportsAsync();
        Task<RequestExportDto> GetRequestExportByIdAsync(int requestId);
    }
}
