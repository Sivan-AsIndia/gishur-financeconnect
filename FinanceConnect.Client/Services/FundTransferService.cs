using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceConnect.Client.Services
{
    public class FundTransferService
    {
        private readonly List<FundTransferModel> _transfers = new();
        private readonly List<BankTransactionModel> _bankLegs = new();

        public FundTransferService()
        {
            // Initialize with some seed data
            _transfers = FundTransferServiceData.Get();
        }

        // ---------------- LIST ----------------
        public List<FundTransferModel> GetList()
            => _transfers.OrderByDescending(x => x.TransferDate).ToList();

        public FundTransferModel? GetById(Guid id)
            => _transfers.FirstOrDefault(x => x.FundTransferId == id);

        // ---------------- CREATE ----------------
        public void Create(FundTransferModel model)
        {
            model.FundTransferNumber = $"FNDTRF-{_transfers.Count + 1:00000}";
            model.Status = FundTransferStatus.Draft;

            _transfers.Add(model);
        }

        public void UpdateDraft(FundTransferModel model)
        {
            var existing = GetById(model.FundTransferId);
            if (existing == null || existing.Status != FundTransferStatus.Draft) return;

            existing.SourceBankAccount = model.SourceBankAccount;
            existing.DestinationBankAccount = model.DestinationBankAccount;
            existing.TransferDate = model.TransferDate;
            existing.SourceValueDate = model.SourceValueDate;
            existing.DestinationValueDate = model.DestinationValueDate;
            existing.Amount = model.Amount;
            existing.Narration = model.Narration;
            existing.TransferMethod = model.TransferMethod;
            existing.Currency = model.Currency;
        }

        // ---------------- WORKFLOW ----------------
        public void Submit(Guid id)
        {
            var t = GetById(id) ?? throw new Exception("Transfer not found");

            ValidateCore(t);

            t.Status = FundTransferStatus.Submitted;
            t.SubmittedOn = DateTime.Now;
        }

        public void Approve(Guid id, string approver)
        {
            var t = GetById(id) ?? throw new Exception("Transfer not found");

            t.Status = FundTransferStatus.Approved;
            t.ApprovedBy = approver;
            t.ApprovedOn = DateTime.Now;
        }

        public void MarkInitiated(Guid id, string utr)
        {
            if (string.IsNullOrWhiteSpace(utr))
                throw new Exception("UTR mandatory");

            var t = GetById(id) ?? throw new Exception("Transfer not found");

            t.UTRNumber = utr;
            t.Status = FundTransferStatus.Initiated;
        }

        public void MarkCompleted(Guid id)
        {
            var t = GetById(id) ?? throw new Exception("Transfer not found");

            t.Status = t.DestinationValueDate > t.SourceValueDate
                ? FundTransferStatus.InTransit
                : FundTransferStatus.Completed;
        }

        // ---------------- POSTING ----------------
        public void Post(Guid id)
        {
            var t = GetById(id) ?? throw new Exception("Transfer not found");

            if (t.Status is not FundTransferStatus.Completed
                and not FundTransferStatus.InTransit
                and not FundTransferStatus.Approved)
                throw new Exception("Cannot post transfer in current status");

            var groupId = Guid.NewGuid();

            // Source leg
            var sourceLeg = new BankTransactionModel
            {
                CompanyName = t.Company,
                BranchName = t.Branch,
                TransactionDate = t.TransferDate,
                ValueDate = t.SourceValueDate,
                Direction = "Outflow",
                Amount = t.Amount,
                PaymentMethod = t.TransferMethod.ToString(),
                UTRNumber = t.UTRNumber,
                SourceModule = "FundTransfer",
                TransactionType = "InterBankTransfer",
                ReferenceNumber = t.FundTransferNumber,
                PostingStatus = "Posted"
            };

            // Destination leg
            var destLeg = new BankTransactionModel
            {
                CompanyName = t.Company,
                BranchName = t.Branch,
                TransactionDate = t.TransferDate,
                ValueDate = t.DestinationValueDate,
                Direction = "Inflow",
                Amount = t.Amount,
                PaymentMethod = t.TransferMethod.ToString(),
                UTRNumber = t.UTRNumber,
                SourceModule = "FundTransfer",
                TransactionType = "InterBankTransfer",
                ReferenceNumber = t.FundTransferNumber,
                PostingStatus = "Posted"
            };

            _bankLegs.Add(sourceLeg);
            _bankLegs.Add(destLeg);

            t.TransferGroupId = groupId;
            t.SourceLegId = sourceLeg.Id;
            t.DestinationLegId = destLeg.Id;

            t.Status = FundTransferStatus.Posted;
        }

        // ---------------- REVERSAL ----------------
        public void Reverse(Guid id, string reason)
        {
            var original = GetById(id) ?? throw new Exception("Transfer not found");

            if (original.Status != FundTransferStatus.Posted)
                throw new Exception("Only posted transfer can be reversed");

            original.Status = FundTransferStatus.Reversed;
            original.ReversalReason = reason;

            var reversal = new FundTransferModel
            {
                FundTransferNumber = $"REV-{original.FundTransferNumber}",
                SourceBankAccount = original.DestinationBankAccount,
                DestinationBankAccount = original.SourceBankAccount,
                Amount = original.Amount,
                Narration = "REVERSAL: " + reason,
                Status = FundTransferStatus.Posted,
                ReversalFundTransferId = original.FundTransferId
            };

            Create(reversal);
            Post(reversal.FundTransferId);
        }

        // ---------------- VALIDATION ----------------
        private static void ValidateCore(FundTransferModel t)
        {
            if (t.SourceBankAccount == t.DestinationBankAccount)
                throw new Exception("Source and destination accounts cannot be the same");

            if (t.Amount <= 0)
                throw new Exception("Amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(t.Narration))
                throw new Exception("Narration is required");
        }

        // ---------------- DELETE ----------------
        public void Delete(Guid id)
        {
            var t = GetById(id);
            if (t != null) _transfers.Remove(t);
        }
    }
}
