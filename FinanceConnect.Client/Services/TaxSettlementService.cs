using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.TaxSettlementViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxSettlementService
    {
        private readonly List<TaxSettlementModel> _settlements;

        public TaxSettlementService()
        {
            _settlements = TaxSettlementSeedData.Get();
        }

        // ── List / Query ──
        public List<TaxSettlementModel> GetList()
            => _settlements.Where(x => !x.IsDeleted)
                           .OrderByDescending(x => x.SettlementDate)
                           .ToList();

        public List<TaxSettlementModel> GetByType(string type)
            => _settlements.Where(x => !x.IsDeleted && x.SettlementType == type)
                           .OrderByDescending(x => x.SettlementDate).ToList();

        public List<TaxSettlementModel> GetByStatus(string status)
            => _settlements.Where(x => !x.IsDeleted && x.SettlementStatus == status)
                           .OrderByDescending(x => x.SettlementDate).ToList();

        public List<TaxSettlementModel> GetByPeriod(string taxPeriodKey)
            => _settlements.Where(x => !x.IsDeleted && x.TaxPeriodKey == taxPeriodKey)
                           .OrderByDescending(x => x.SettlementDate).ToList();

        public TaxSettlementModel? GetById(Guid id)
            => _settlements.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        // ── Create ──
        public void Create(TaxSettlementModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SettlementType))
                throw new Exception("Settlement Type is required.");
            if (string.IsNullOrWhiteSpace(model.TaxPeriodKey))
                throw new Exception("Tax Period is required.");
            if (string.IsNullOrWhiteSpace(model.TaxTypeScope))
                throw new Exception("Tax Type Scope is required.");

            model.SettlementNumber = $"TAXSET-{DateTime.Now.Year}-{(_settlements.Count + 1):00000}";
            model.SettlementStatus = "Draft";
            model.CreatedAt = DateTime.Now;
            model.ReconciliationStatus = "NotReconciled";
            RecalculateTotals(model);

            _settlements.Add(model);
        }

        // ── Update Draft ──
        public void UpdateDraft(TaxSettlementModel model)
        {
            var existing = GetById(model.Id);
            if (existing == null) throw new Exception("Settlement not found.");
            if (existing.SettlementStatus != "Draft")
                throw new Exception("Only Draft settlements can be edited.");

            existing.SettlementType = model.SettlementType;
            existing.SettlementDate = model.SettlementDate;
            existing.PostingDate = model.PostingDate;
            existing.Narration = model.Narration;
            existing.AccountingPeriodId = model.AccountingPeriodId;
            existing.AccountingPeriodName = model.AccountingPeriodName;
            existing.TaxPeriodKey = model.TaxPeriodKey;
            existing.TaxTypeScope = model.TaxTypeScope;
            existing.GovernmentAuthorityType = model.GovernmentAuthorityType;
            existing.JurisdictionCode = model.JurisdictionCode;
            existing.PaymentMode = model.PaymentMode;
            existing.BankAccountId = model.BankAccountId;
            existing.BankAccountName = model.BankAccountName;
            existing.CashAccountId = model.CashAccountId;
            existing.CashAccountName = model.CashAccountName;
            existing.ChallanNumber = model.ChallanNumber;
            existing.ChallanDate = model.ChallanDate;
            existing.GovernmentReferenceNumber = model.GovernmentReferenceNumber;
            existing.PaymentReferenceNumber = model.PaymentReferenceNumber;
            existing.RemittedOn = model.RemittedOn;
            existing.InputCreditOffsetCGSTAmount = model.InputCreditOffsetCGSTAmount;
            existing.InputCreditOffsetSGSTAmount = model.InputCreditOffsetSGSTAmount;
            existing.InputCreditOffsetIGSTAmount = model.InputCreditOffsetIGSTAmount;
            existing.InputCreditOffsetCESSAmount = model.InputCreditOffsetCESSAmount;
            existing.SettlementNotes = model.SettlementNotes;
            existing.Allocations = model.Allocations;
            existing.UpdatedAt = DateTime.Now;

            RecalculateTotals(existing);
        }

        // ── Workflow ──
        public void Submit(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Draft") throw new Exception("Only Draft can be submitted.");
            if (s.AllocationCount == 0) throw new Exception("At least one allocation line is required.");
            s.SettlementStatus = "Submitted";
            s.UpdatedAt = DateTime.Now;
        }

        public void Approve(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Submitted") throw new Exception("Only Submitted can be approved.");
            s.SettlementStatus = "Approved";
            s.UpdatedAt = DateTime.Now;
        }

        public void Reject(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Submitted") throw new Exception("Only Submitted can be rejected.");
            s.SettlementStatus = "Draft";
            s.UpdatedAt = DateTime.Now;
        }

        public void Post(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Approved") throw new Exception("Only Approved can be posted.");

            ValidateForPosting(s);

            s.SettlementStatus = "Posted";
            s.PostingDate ??= DateTime.Now;
            s.PostedOn = DateTime.Now;
            s.PostedBy = "admin@acme.com";
            s.JournalEntryId = Guid.NewGuid();

            foreach (var alloc in s.Allocations)
            {
                alloc.AllocationStatus = "Applied";
                alloc.OutstandingAfterAllocation = alloc.OutstandingBeforeAllocation - alloc.AllocatedAmount;
            }
            s.UpdatedAt = DateTime.Now;
        }

        public void Close(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Posted" && s.SettlementStatus != "Reconciled")
                throw new Exception("Only Posted/Reconciled can be closed.");
            s.SettlementStatus = "Closed";
            s.ClosedOn = DateTime.Now;
            s.ClosedBy = "controller@acme.com";
            s.ReconciliationStatus = "Reconciled";
            s.IsFullyReconciled = true;
            s.UpdatedAt = DateTime.Now;
        }

        public void Reverse(Guid id, string reason)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Posted") throw new Exception("Only Posted can be reversed.");
            if (string.IsNullOrWhiteSpace(reason)) throw new Exception("Reversal reason is required.");
            s.SettlementStatus = "Reversed";
            s.ReversalReason = reason;
            s.ReversalJournalEntryId = Guid.NewGuid();
            foreach (var alloc in s.Allocations)
            {
                alloc.AllocationStatus = "Reversed";
                alloc.OutstandingAfterAllocation = alloc.OutstandingBeforeAllocation;
            }
            s.RemainingUnsettledAmount = s.TotalOutstandingAmount;
            s.IsFullyAllocated = false;
            s.UpdatedAt = DateTime.Now;
        }

        public void Delete(Guid id)
        {
            var s = GetById(id) ?? throw new Exception("Settlement not found.");
            if (s.SettlementStatus != "Draft") throw new Exception("Only Draft can be deleted.");
            s.IsDeleted = true;
        }

        // ── Helpers ──
        private void RecalculateTotals(TaxSettlementModel s)
        {
            s.AllocationCount = s.Allocations.Count;
            s.TotalSettlementAmount = s.Allocations.Sum(a => a.AllocatedAmount);
            s.TotalCashOrBankPaidAmount = s.Allocations.Where(a => a.SettlementMode == "CashOrBank").Sum(a => a.AllocatedAmount);
            s.TotalCreditOffsetAmount = s.Allocations.Where(a => a.SettlementMode == "CreditOffset").Sum(a => a.AllocatedAmount);
            s.RemainingUnsettledAmount = s.TotalOutstandingAmount - s.TotalSettlementAmount;
            s.IsFullyAllocated = s.RemainingUnsettledAmount <= 0;
        }

        private void ValidateForPosting(TaxSettlementModel s)
        {
            if (s.Allocations.Count == 0)
                throw new Exception("At least one allocation line is required.");
            if (s.Allocations.Any(a => a.AllocatedAmount <= 0))
                throw new Exception("All allocation amounts must be greater than 0.");
            if (s.Allocations.Any(a => a.AllocatedAmount > a.OutstandingBeforeAllocation))
                throw new Exception("Allocated amount cannot exceed outstanding amount.");
            if (s.PaymentMode == "Bank" && s.BankAccountId == null)
                throw new Exception("Bank Account is required for Bank payment mode.");
            if (s.PaymentMode == "Cash" && s.CashAccountId == null)
                throw new Exception("Cash Account is required for Cash payment mode.");
        }
    }
}
