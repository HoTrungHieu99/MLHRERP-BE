using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _repository;

        public PaymentTransactionService(IPaymentTransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PaymentTransactionDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(ToDto);
        }

        public async Task<PaymentTransactionDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<IEnumerable<PaymentTransactionDto>> GetByUserIdAsync(Guid userId)
        {
            var entities = await _repository.GetByUserIdAsync(userId);
            return entities.Select(ToDto);
        }

        private PaymentTransactionDto ToDto(PaymentTransaction t)
        {
            return new PaymentTransactionDto
            {
                TransactionId = t.TransactionId,
                PaymentDate = t.PaymentDate,
                Amount = t.Amount,
                PaymentStatus = t.PaymentStatus,
                TransactionReference = t.TransactionReference
            };
        }
    }

}
