using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseClaimViewModel;

namespace FinanceConnect.Client.Services
{
    public class ExpenseClaimService
    {
        private List<ExpenseClaim> _items;
        public ExpenseClaimService() { _items = ExpenseClaimSeedData.GetAll(); }

        public List<ExpenseClaim> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public ExpenseClaim? GetById(Guid id) => _items.FirstOrDefault(x => x.ExpenseClaimId == id && !x.IsDeleted);
        public Task<List<ExpenseClaim>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<ExpenseClaim?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public void Add(ExpenseClaim m)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == m.CompanyId && string.Equals(x.ClaimCode, m.ClaimCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Claim Code '{m.ClaimCode}' already exists.");
            m.ExpenseClaimId = Guid.NewGuid(); m.CreatedAt = DateTime.UtcNow; m.IsDeleted = false;
            foreach (var l in m.Lines) { l.ExpenseClaimLineId = Guid.NewGuid(); l.ExpenseClaimId = m.ExpenseClaimId; }
            _items.Add(m);
        }
        public Task CreateAsync(ExpenseClaim m) { Add(m); return Task.CompletedTask; }

        public void Update(ExpenseClaim m)
        {
            var e = GetById(m.ExpenseClaimId); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked claim cannot be edited.");
            if (e.ClaimStatus == ClaimStatusEnum.Reimbursed || e.ClaimStatus == ClaimStatusEnum.Closed) throw new InvalidOperationException("Reimbursed or closed claim cannot be edited.");
            e.ClaimTitle=m.ClaimTitle; e.Description=m.Description; e.ClaimStatus=m.ClaimStatus;
            e.ClaimantEmployeeId=m.ClaimantEmployeeId; e.ClaimantCodeSnapshot=m.ClaimantCodeSnapshot; e.ClaimantNameSnapshot=m.ClaimantNameSnapshot;
            e.ClaimSubmissionDate=m.ClaimSubmissionDate; e.ClaimPeriodFrom=m.ClaimPeriodFrom; e.ClaimPeriodTo=m.ClaimPeriodTo; e.BusinessPurpose=m.BusinessPurpose;
            e.DepartmentId=m.DepartmentId; e.DepartmentName=m.DepartmentName; e.BranchId=m.BranchId; e.BranchName=m.BranchName;
            e.CostCenterId=m.CostCenterId; e.CostCenterName=m.CostCenterName; e.ProjectId=m.ProjectId; e.ProjectName=m.ProjectName; e.ManagerApproverId=m.ManagerApproverId;
            e.CurrencyId=m.CurrencyId; e.ExchangeRateId=m.ExchangeRateId;
            e.TotalClaimedAmount=m.TotalClaimedAmount; e.TotalApprovedAmount=m.TotalApprovedAmount; e.TotalRejectedAmount=m.TotalRejectedAmount; e.TotalReimbursedAmount=m.TotalReimbursedAmount;
            e.ReimbursementStatus=m.ReimbursementStatus; e.ReceiptComplianceStatus=m.ReceiptComplianceStatus; e.PolicyCheckStatus=m.PolicyCheckStatus; e.DuplicateCheckStatus=m.DuplicateCheckStatus;
            e.PolicyOverrideReason=m.PolicyOverrideReason; e.ReceiptRequiredFlag=m.ReceiptRequiredFlag;
            e.SubmittedByUserId=m.SubmittedByUserId; e.SubmittedOn=m.SubmittedOn; e.ReviewedByUserId=m.ReviewedByUserId; e.ReviewedOn=m.ReviewedOn;
            e.ApprovedByUserId=m.ApprovedByUserId; e.ApprovedOn=m.ApprovedOn; e.RejectedByUserId=m.RejectedByUserId; e.RejectedOn=m.RejectedOn;
            e.ApprovalNotes=m.ApprovalNotes; e.CancellationReason=m.CancellationReason;
            e.ReimbursementMethod=m.ReimbursementMethod; e.ReimbursementDate=m.ReimbursementDate; e.ReimbursementReferenceNumber=m.ReimbursementReferenceNumber;
            e.PostingStatus=m.PostingStatus; e.Notes=m.Notes; e.SupportingCommentary=m.SupportingCommentary;
            e.PolicyExceptionFlag=m.PolicyExceptionFlag; e.PolicyExceptionApprovedBy=m.PolicyExceptionApprovedBy;
            e.Lines = m.Lines;
            e.UpdatedAt = DateTime.UtcNow;
        }
        public Task UpdateAsync(ExpenseClaim m) { Update(m); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked claim cannot be deleted.");
            if (e.ClaimStatus == ClaimStatusEnum.Reimbursed || e.ClaimStatus == ClaimStatusEnum.Closed) throw new InvalidOperationException("Reimbursed/closed claim cannot be deleted.");
            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow;
        }
        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId) + 1;
            return $"CLM-2026-{count:D4}";
        }
    }
}
