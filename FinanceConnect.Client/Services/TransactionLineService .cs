using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceConnect.Client.Services
{
    public class TransactionLineService
    {
        private readonly List<TransactionLineModel> _store = new();

        // ================= READ =================
        public List<TransactionLineModel> GetByTransaction(Guid transactionId)
        {
            return _store
                .Where(x => x.FinancialTransactionId == transactionId)
                .OrderBy(x => x.LineNumber)
                .ToList();
        }

        // ================= CREATE =================
        public TransactionLineModel Add(TransactionLineModel line)
        {
            if (line.FinancialTransactionId == Guid.Empty)
                throw new InvalidOperationException("Parent transaction is required.");

            if (line.LineAmount == 0)
                throw new InvalidOperationException("Line amount cannot be zero.");

            if (_store.Any(x =>
                x.FinancialTransactionId == line.FinancialTransactionId &&
                x.LineNumber == line.LineNumber))
                throw new InvalidOperationException("Duplicate line number.");

            line.TransactionLineId = Guid.NewGuid();
            line.CreatedAt = DateTime.UtcNow;

            _store.Add(line);
            return line;
        }

        // ================= UPDATE =================
        public void Update(TransactionLineModel line)
        {
            var existing = _store.FirstOrDefault(x => x.TransactionLineId == line.TransactionLineId);
            if (existing == null)
                throw new InvalidOperationException("Line not found.");

            if (existing.IsSystemGenerated)
                throw new InvalidOperationException("System-generated lines cannot be edited.");

            existing.LineType = line.LineType;
            existing.PostingCategory = line.PostingCategory;
            existing.LineNarration = line.LineNarration;
            existing.Quantity = line.Quantity;
            existing.UnitRate = line.UnitRate;
            existing.LineAmount = line.LineAmount;
            existing.BaseAmount = line.BaseAmount;
            existing.IsTaxLine = line.IsTaxLine;
            existing.TaxCodeId = line.TaxCodeId;
            existing.TaxComponentType = line.TaxComponentType;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        // ================= DELETE =================
        public void Delete(Guid lineId)
        {
            var line = _store.FirstOrDefault(x => x.TransactionLineId == lineId);
            if (line == null)
                return;

            if (line.IsSystemGenerated)
                throw new InvalidOperationException("System-generated lines cannot be deleted.");

            _store.Remove(line);
        }

        // ================= TOTALS =================
        public decimal GetTotal(Guid transactionId)
        {
            return _store
                .Where(x => x.FinancialTransactionId == transactionId)
                .Sum(x => x.LineAmount);
        }

        public void SeedLine(TransactionLineModel line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            ValidateSeedLine(line);

            // Prevent duplicate seeding
            bool exists = _store.Any(l =>
                l.FinancialTransactionId == line.FinancialTransactionId &&
                l.LineNumber == line.LineNumber);

            if (exists)
                return;

            // Auto line number if not provided
            if (line.LineNumber <= 0)
            {
                line.LineNumber = GetNextLineNumber(line.FinancialTransactionId);
            }

            // System flags
            line.IsSystemGenerated = true;
            line.CreatedAt = DateTime.UtcNow;

            _store.Add(line);
        }

        private void ValidateSeedLine(TransactionLineModel line)
        {
            if (line.FinancialTransactionId == Guid.Empty)
                throw new InvalidOperationException("FinancialTransactionId is required");

            if (!line.PostingCategory.HasValue)
                throw new InvalidOperationException("LineType is required");

            if (!line.PostingCategory.HasValue)
                throw new InvalidOperationException("PostingCategory is required");

            if (line.BranchId == Guid.Empty)
                throw new InvalidOperationException("BranchId is required");

            if (line.LineAmount == 0)
                throw new InvalidOperationException("LineAmount cannot be zero");
        }

        private int GetNextLineNumber(Guid transactionId)
        {
            var max = _store
                .Where(l => l.FinancialTransactionId == transactionId)
                .Select(l => l.LineNumber)
                .DefaultIfEmpty(0)
                .Max();

            // Enterprise pattern: 10, 20, 30
            return max + 10;
        }
    }
}
