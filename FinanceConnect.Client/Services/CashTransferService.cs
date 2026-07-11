using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceConnect.Client.Services
{
    public class CashTransferService
    {
        private readonly List<CashTransferModel> Transaction = new();

        // Get by ID
        public CashTransferService()
        {
            Transaction = CashTransferServiceData.Get();
        }
        public Task<CashTransferModel?> GetByIdAsync(Guid id)
        {
            var transfer = Transaction.FirstOrDefault(t => t.CashTransferId == id);
            return Task.FromResult(transfer);
        }

        // Get all
        public Task<List<CashTransferModel>> GetListAsync()
        {
            return Task.FromResult(Transaction);
        }

        // Create
        public Task<CashTransferModel> CreateAsync(CashTransferModel model)
        {
            model.CashTransferId = Guid.NewGuid();
            model.TransferDate = DateTime.Now;

            Transaction.Add(model);
            return Task.FromResult(model);
        }

        // Update
        public Task<bool> UpdateAsync(CashTransferModel model)
        {
            var existing = Transaction.FirstOrDefault(t => t.CashTransferId == model.CashTransferId);
            if (existing == null) return Task.FromResult(false);

            existing.CashTransferNumber = model.CashTransferNumber;
            existing.TransferDate = model.TransferDate;
            existing.SourceCashAccountName = model.SourceCashAccountName;
            existing.DestinationCashAccountName = model.DestinationCashAccountName;
            existing.Amount = model.Amount;
            existing.BranchName = model.BranchName;
            existing.CashTransferStatus = model.CashTransferStatus;
            existing.PostingStatus = model.PostingStatus;
            existing.CurrencyCode = model.CurrencyCode;

            return Task.FromResult(true);
        }

        // Delete
        public Task<bool> DeleteAsync(Guid id)
        {
            var existing = Transaction.FirstOrDefault(t => t.CashTransferId == id);
            if (existing == null) return Task.FromResult(false);

            Transaction.Remove(existing);
            return Task.FromResult(true);
        }
    }

}
