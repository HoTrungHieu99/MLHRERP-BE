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

            var dueDate = payment.CreatedAt.AddMonths(3);
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
                UpdatedAt = payment.UpdatedAt,
                TransactionReference = payment.PaymentTransactions?.FirstOrDefault()?.TransactionReference ?? "N/A", // 👈 Lấy TransactionReference
                DueDate = dueDate,
                DebtStatus = GetDebtStatus(dueDate)
            };
        }


        public async Task<List<PaymentHistoryDto>> GetAllPaymentHistoriesAsync()
        {
            var histories = await _repository.GetAllAsync();

            return histories.Select(ph =>
            {
                var dueDate = ph.CreatedAt.AddMonths(3);
                return new PaymentHistoryDto
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
                    UpdatedAt = ph.UpdatedAt,
                    TransactionReference = ph.PaymentTransactions?.FirstOrDefault()?.TransactionReference ?? "N/A",
                    DueDate = dueDate,
                    DebtStatus = GetDebtStatus(dueDate)
                };
            }).ToList();
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentHistoriesByUserIdAsync(Guid userId)
        {
            var payments = await _repository.GetPaymentHistoryByUserIdAsync(userId);

            return payments.Select(ph =>
            {
                var dueDate = ph.CreatedAt.AddMonths(3);
                return new PaymentHistoryDto
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
                    UpdatedAt = ph.UpdatedAt,
                    TransactionReference = ph.PaymentTransactions?.FirstOrDefault()?.TransactionReference ?? "N/A",
                    DueDate = dueDate,
                    DebtStatus = GetDebtStatus(dueDate)
                };
            }).ToList();
        }

        private string GetDebtStatus(DateTime dueDate)
        {
            var daysLeft = (dueDate - DateTime.UtcNow).TotalDays;

            if (daysLeft > 10)
                return "StillValid";
            else if (daysLeft <= 10 && daysLeft >= 0)
                return "NearDue";
            else
                return "OverDue";
        }

    }
}
