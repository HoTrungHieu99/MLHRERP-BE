using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IImageRepository
    {
        Task<Image> AddAsync(Image image);
        Task SaveChangesAsync();
    }
}
