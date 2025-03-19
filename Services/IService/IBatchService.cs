using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IBatchService
    {
        Task<Batch> GetBatchByIdAsync(long batchId);
        Task<IEnumerable<Batch>> GetAllBatchesAsync();
        Task<bool> UpdateBatchAsync(Batch batch);
        Task<IEnumerable<Batch>> GetBatchesByProductIdAsync(long productId);
    }
}
