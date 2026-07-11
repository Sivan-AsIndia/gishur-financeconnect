using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.RevenueViewModel;

namespace FinanceConnect.Client.Services
{
    public class RevenueService
    {
        private List<Revenue> _items = new();

        public RevenueService()
        {
            _items = RevenueSeedData.GetAll();
        }

        // ── Query ──────────────────────────────────────────────────────────────

        public List<Revenue> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public Revenue? GetById(Guid id)
            => _items.FirstOrDefault(x => x.RevenueId == id && !x.IsDeleted);

        public Task<List<Revenue>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<Revenue?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        // ── Create ─────────────────────────────────────────────────────────────

        public void Add(Revenue model)
        {
            if (_items.Any(x => !x.IsDeleted &&
                x.CompanyId == model.CompanyId &&
                string.Equals(x.RevenueCode, model.RevenueCode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Revenue Code '{model.RevenueCode}' already exists for this company.");
            }

            if (model.IsLocked)
                throw new InvalidOperationException("Cannot create a revenue record in locked state.");

            model.RevenueId  = Guid.NewGuid();
            model.CreatedAt  = DateTime.UtcNow;
            model.IsDeleted  = false;
            _items.Add(model);
        }

        public Task CreateAsync(Revenue model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        // ── Update ─────────────────────────────────────────────────────────────

        public void Update(Revenue model)
        {
            var existing = GetById(model.RevenueId);
            if (existing is null) return;

            if (existing.IsLocked)
                throw new InvalidOperationException("Locked revenue record cannot be edited.");

            if (existing.Status == RevenueStatus.Closed ||
                existing.Status == RevenueStatus.FullyRecognized)
                throw new InvalidOperationException(
                    "Closed or fully recognised revenue cannot be materially changed.");

            existing.RevenueName                     = model.RevenueName;
            existing.Description                     = model.Description;
            existing.Status                          = model.Status;
            existing.CustomerId                      = model.CustomerId;
            existing.CustomerCodeSnapshot            = model.CustomerCodeSnapshot;
            existing.CustomerNameSnapshot            = model.CustomerNameSnapshot;
            existing.RevenueSourceDocType              = model.RevenueSourceDocType;
            existing.SourceDocumentNumber            = model.SourceDocumentNumber;
            existing.ContractId                      = model.ContractId;
            existing.ContractReference               = model.ContractReference;
            existing.SubscriptionId                  = model.SubscriptionId;
            existing.SubscriptionReference           = model.SubscriptionReference;
            existing.ProjectId                       = model.ProjectId;
            existing.ProjectReference                = model.ProjectReference;
            existing.MilestoneReference              = model.MilestoneReference;
            existing.RevenueType                     = model.RevenueType;
            existing.RevenueCategoryCode             = model.RevenueCategoryCode;
            existing.GLAccountId                     = model.GLAccountId;
            existing.GLAccountName                   = model.GLAccountName;
            existing.RevenueNature                   = model.RevenueNature;
            existing.BusinessEventDate               = model.BusinessEventDate;
            existing.OperationalPeriodFrom           = model.OperationalPeriodFrom;
            existing.OperationalPeriodTo             = model.OperationalPeriodTo;
            existing.GrossRevenueAmount              = model.GrossRevenueAmount;
            existing.TaxExclusiveRevenueAmount       = model.TaxExclusiveRevenueAmount;
            existing.AdjustmentAmount                = model.AdjustmentAmount;
            existing.CurrencyId                      = model.CurrencyId;
            existing.ExchangeRateId                  = model.ExchangeRateId;
            existing.RecognitionMethod               = model.RecognitionMethod;
            existing.RecognitionStartDate            = model.RecognitionStartDate;
            existing.RecognitionEndDate              = model.RecognitionEndDate;
            existing.RecognitionFrequency            = model.RecognitionFrequency;
            existing.RecognitionStatus               = model.RecognitionStatus;
            existing.IsRecognitionRequired           = model.IsRecognitionRequired;
            existing.IsDeferredRevenueRequired       = model.IsDeferredRevenueRequired;
            existing.DeferredRevenueId               = model.DeferredRevenueId;
            existing.DeferredRevenueReference        = model.DeferredRevenueReference;
            existing.RevenueRecognitionTemplateCode  = model.RevenueRecognitionTemplateCode;
            existing.BillingStatus                   = model.BillingStatus;
            existing.CollectionStatus                = model.CollectionStatus;
            existing.InvoiceId                       = model.InvoiceId;
            existing.InvoiceReference                = model.InvoiceReference;
            existing.InvoiceNumberSnapshot           = model.InvoiceNumberSnapshot;
            existing.BillingDate                     = model.BillingDate;
            existing.CollectionReferenceText         = model.CollectionReferenceText;
            existing.IsAdvanceReceipt                = model.IsAdvanceReceipt;
            existing.BranchId                        = model.BranchId;
            existing.BranchName                      = model.BranchName;
            existing.DepartmentId                    = model.DepartmentId;
            existing.DepartmentName                  = model.DepartmentName;
            existing.CostCenterId                    = model.CostCenterId;
            existing.CostCenterName                  = model.CostCenterName;
            existing.RevenueOwnerUserId              = model.RevenueOwnerUserId;
            existing.RevenueOwnerUserText            = model.RevenueOwnerUserText;
            existing.BusinessUnitCode                = model.BusinessUnitCode;
            existing.DimensionScopeJson              = model.DimensionScopeJson;
            existing.PreparedByUserId                = model.PreparedByUserId;
            existing.ReviewedByUserId                = model.ReviewedByUserId;
            existing.ApprovedByUserId                = model.ApprovedByUserId;
            existing.PreparedOn                      = model.PreparedOn;
            existing.ReviewedOn                      = model.ReviewedOn;
            existing.ApprovedOn                      = model.ApprovedOn;
            existing.CancellationReason              = model.CancellationReason;
            existing.RevenueAssumptionText           = model.RevenueAssumptionText;
            existing.Notes                           = model.Notes;
            existing.AttachmentCount                 = model.AttachmentCount;
            existing.UpdatedAt                       = DateTime.UtcNow;
        }

        public Task UpdateAsync(Revenue model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        // ── Delete (soft) ──────────────────────────────────────────────────────

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;

            if (item.IsLocked)
                throw new InvalidOperationException("Locked revenue cannot be deleted.");

            if (item.Status == RevenueStatus.FullyRecognized ||
                item.Status == RevenueStatus.Closed)
                throw new InvalidOperationException(
                    "Fully recognised or closed revenue cannot be deleted.");

            item.IsDeleted  = true;
            item.UpdatedAt  = DateTime.UtcNow;
        }

        public Task DeleteAsync(Guid id)
        {
            Delete(id);
            return Task.CompletedTask;
        }

        // ── Lock / Unlock ──────────────────────────────────────────────────────

        public void Lock(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;
            item.IsLocked  = true;
            item.LockedOn  = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
        }

        public void Unlock(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;
            item.IsLocked  = false;
            item.LockedOn  = null;
            item.UpdatedAt = DateTime.UtcNow;
        }

        // ── Reset ──────────────────────────────────────────────────────────────

        public void ResetToSeed()
        {
            _items = RevenueSeedData.GetAll();
        }

        // ── Auto-generate Revenue Code ─────────────────────────────────────────

        public string GenerateCode(Guid companyId)
        {
            var year  = DateTime.Today.Year;
            var count = _items.Count(x => x.CompanyId == companyId && !x.IsDeleted) + 1;
            return $"REV-{year}-{count:D4}";
        }
    }
}
