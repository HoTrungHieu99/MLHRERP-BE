using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;
using Repo.IRepository;
using Repo.Repository;
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

        public async Task<PaymentHistoryDto> GetPaymentHistoryByIdAsync(Guid id)
        {
            var payment = await _repository.GetByIdAsync(id);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lịch sử thanh toán với ID = {id}");
            }

            return new PaymentHistoryDto
            {
                PaymentHistoryId = payment.PaymentHistoryId,
                OrderId = payment.OrderId,
                OrderCode = payment.Order?.OrderCode ?? "N/A",

                AgencyId = payment.Order?.RequestProduct?.AgencyId ?? 0,
                AgencyName = payment.Order?.RequestProduct?.AgencyAccount?.AgencyName ?? "Unknown",

                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                SerieNumber = payment.SerieNumber,
                Status = payment.Status,
                TotalAmountPayment = payment.TotalAmountPayment,
                RemainingDebtAmount = payment.RemainingDebtAmount,
                PaymentAmount = payment.PaymentAmount,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }


        public async Task<List<PaymentHistoryDto>> GetAllPaymentHistoriesAsync()
        {
            var histories = await _repository.GetAllAsync();

            return histories.Select(ph => new PaymentHistoryDto
            {
                PaymentHistoryId = ph.PaymentHistoryId,
                OrderId = ph.OrderId,
                OrderCode = ph.Order?.OrderCode ?? "N/A",

                AgencyId = ph.Order?.RequestProduct?.AgencyId ?? 0,
                AgencyName = ph.Order?.RequestProduct?.AgencyAccount?.AgencyName ?? "Unknown",

                PaymentMethod = ph.PaymentMethod,
                PaymentDate = ph.PaymentDate,
                SerieNumber = ph.SerieNumber,
                Status = ph.Status,
                TotalAmountPayment = ph.TotalAmountPayment,
                RemainingDebtAmount = ph.RemainingDebtAmount,
                PaymentAmount = ph.PaymentAmount,
                CreatedAt = ph.CreatedAt,
                UpdatedAt = ph.UpdatedAt
            }).ToList();
        }

    }
}
