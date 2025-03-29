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
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IPaymentHistoryRepository _repository;

        public PaymentHistoryService(IPaymentHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentHistory> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<PaymentHistory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
