using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class BatchService : IBatchService
    {
        private readonly IBatchRepository _batchRepository;

        public BatchService(IBatchRepository batchRepository)
        {
            _batchRepository = batchRepository;
        }

        public async Task<Batch> GetBatchByIdAsync(long batchId)
        {
            return await _batchRepository.GetByIdAsync(batchId);
        }

        public async Task<IEnumerable<Batch>> GetAllBatchesAsync()
        {
            return await _batchRepository.GetAllAsync();
        }

        public async Task<bool> UpdateBatchAsync(Batch batch)
        {
            return await _batchRepository.UpdateAsync(batch);
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductIdAsync(long productId)
        {
            return await _batchRepository.GetBatchesByProductIdAsync(productId);
        }
    }
}
