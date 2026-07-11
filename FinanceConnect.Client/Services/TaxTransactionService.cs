using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System.Xml.Linq;
using static FinanceConnect.Client.ViewModels.TaxTranactionViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxTransactionService
    {
        private readonly List<TaxTransactionModel> _transactions = new();
        private readonly List<TaxTransactionLineModel> _lines = new();

        public TaxTransactionService()
        {
            _transactions = TaxTransactionSeedData.Get();

            foreach (var tx in _transactions)
                _lines.AddRange(tx.Lines);
        }

        public List<TaxTransactionModel> GetList()
            => _transactions.OrderByDescending(x => x.PostingDate).ToList();

        public List<TaxTransactionModel> GetListByType(string taxType)
            => _transactions
                .Where(x => x.TaxType == taxType)
                .OrderByDescending(x => x.PostingDate)
                .ToList();

        public List<TaxTransactionModel> GetListByPeriod(string taxPeriodKey)
            => _transactions
                .Where(x => x.TaxPeriodKey == taxPeriodKey)
                .OrderByDescending(x => x.PostingDate)
                .ToList();

        public List<TaxTransactionModel> GetListByParty(Guid partyId)
            => _transactions
                .Where(x => x.PartyId == partyId)
                .OrderByDescending(x => x.PostingDate)
                .ToList();

        public List<TaxTransactionModel> GetListBySourceDoc(string sourceDocType, Guid sourceDocId)
            => _transactions
                .Where(x => x.SourceDocumentType == sourceDocType
                         && x.SourceDocumentId == sourceDocId)
                .ToList();

        public TaxTransactionModel? GetById(Guid id)
            => _transactions.FirstOrDefault(x => x.Id == id);

        public TaxTransactionModel? GetByNumber(string taxTransactionNumber)
            => _transactions.FirstOrDefault(x =>
                string.Equals(x.TaxTransactionNumber, taxTransactionNumber,
                              StringComparison.OrdinalIgnoreCase));


        public List<TaxTransactionLineModel> GetLines(Guid taxTransactionId)
        {
            var tx = GetById(taxTransactionId)
                ?? throw new Exception("Tax Transaction not found");
            return tx.Lines;
        }

        public TaxTransactionLineModel? GetLineById(Guid lineId)
            => _lines.FirstOrDefault(l => l.Id == lineId);


        public void Create(TaxTransactionModel model)
        {
            ValidateCreate(model);

            model.TaxTransactionNumber = $"TAX-{DateTime.Now.Year}-{_transactions.Count + 1:00000}";
            model.TaxTransactionStatus = TaxTransactionStatus.Draft;
            model.CreatedAt = DateTime.Now;

            _transactions.Add(model);

            foreach (var line in model.Lines)
            {
                line.TaxTransactionId = model.Id;
                line.CreatedAt = DateTime.Now;
                _lines.Add(line);
            }
        }

        public void UpdateDraft(TaxTransactionModel model)
        {
            var existing = GetById(model.Id);
            if (existing == null || existing.TaxTransactionStatus != TaxTransactionStatus.Draft)
                return;

            existing.SourceDocumentType = model.SourceDocumentType;
            existing.SourceDocumentId = model.SourceDocumentId;
            existing.SourceDocumentNumberSnapshot = model.SourceDocumentNumberSnapshot;
            existing.SourceDocumentDateSnapshot = model.SourceDocumentDateSnapshot;
            existing.PostingDate = model.PostingDate;
            existing.TaxPeriodKey = model.TaxPeriodKey;
            existing.PartyType = model.PartyType;
            existing.PartyId = model.PartyId;
            existing.PartyNameSnapshot = model.PartyNameSnapshot;
            existing.PartyGSTINSnapshot = model.PartyGSTINSnapshot;
            existing.PartyPANSnapshot = model.PartyPANSnapshot;
            existing.SupplyType = model.SupplyType;
            existing.FromStateCode = model.FromStateCode;
            existing.ToStateCode = model.ToStateCode;
            existing.IsReverseCharge = model.IsReverseCharge;
            existing.UpdatedAt = DateTime.Now;
        }

        public void Post(Guid id, string postedBy = "system")
        {
            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            if (tx.TaxTransactionStatus != TaxTransactionStatus.Draft)
                throw new Exception($"Cannot post a transaction in '{tx.TaxTransactionStatus}' status");

            ValidateForPosting(tx);
            RecalculateTotals(tx);

            tx.TaxTransactionStatus = TaxTransactionStatus.Posted;
            tx.PostedOn = DateTime.Now;
            tx.PostedBy = postedBy;
            tx.UpdatedAt = DateTime.Now;

            foreach (var line in tx.Lines)
                line.LineStatus = TaxTransactionLineStatus.Posted;
        }

        public void ExcludeFromReturn(Guid id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Exclusion reason is required");

            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            if (tx.TaxTransactionStatus != TaxTransactionStatus.Posted)
                throw new Exception("Only Posted transactions can be excluded from return");

            tx.ReturnInclusionStatus = ReturnInclusionStatus.Excluded;
            tx.ExclusionReason = reason;
            tx.IsIncludedInReturn = false;
            tx.UpdatedAt = DateTime.Now;
        }

        public void IncludeInReturn(Guid id, Guid? gstReturnRunId = null)
        {
            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            if (tx.TaxTransactionStatus != TaxTransactionStatus.Posted)
                throw new Exception("Only Posted transactions can be included in return");

            if (tx.ReturnInclusionStatus == ReturnInclusionStatus.Included)
                throw new Exception("Transaction is already included in a return");

            tx.IsIncludedInReturn = true;
            tx.ReturnInclusionStatus = ReturnInclusionStatus.Included;
            tx.GSTReturnRunId = gstReturnRunId;
            tx.ExclusionReason = null;
            tx.TaxTransactionStatus = TaxTransactionStatus.IncludedInReturn;
            tx.UpdatedAt = DateTime.Now;
        }

        public void MarkSettled(Guid id, bool partial = false)
        {
            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            tx.TaxTransactionStatus = partial
                ? TaxTransactionStatus.PartiallySettled
                : TaxTransactionStatus.Settled;
            tx.UpdatedAt = DateTime.Now;
        }

        public void MarkReconciled(Guid id, string? notes = null)
        {
            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            tx.ReconciliationStatus = ReconciliationStatus1.Matched;
            if (!string.IsNullOrWhiteSpace(notes))
                tx.ReconciliationNotes = notes;
            tx.UpdatedAt = DateTime.Now;
        }

        public void MarkMismatch(Guid id, string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                throw new Exception("Mismatch notes are required");

            var tx = GetById(id) ?? throw new Exception("Tax Transaction not found");

            tx.ReconciliationStatus = ReconciliationStatus1.Mismatch;
            tx.ReconciliationNotes = notes;
            tx.UpdatedAt = DateTime.Now;
        }

        public void Reverse(Guid id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Reversal reason is required");

            var original = GetById(id) ?? throw new Exception("Tax Transaction not found");

            if (original.TaxTransactionStatus is not TaxTransactionStatus.Posted
                                              and not TaxTransactionStatus.IncludedInReturn
                                              and not TaxTransactionStatus.Settled)
                throw new Exception("Only Posted / IncludedInReturn / Settled transactions can be reversed");

            original.TaxTransactionStatus = TaxTransactionStatus.Reversed;
            original.ReversalReason = reason;
            original.UpdatedAt = DateTime.Now;

            var reversal = new TaxTransactionModel
            {
                TenantId = original.TenantId,
                CompanyId = original.CompanyId,
                CompanyName = original.CompanyName,
                BranchId = original.BranchId,
                BranchName = original.BranchName,
                TaxType = original.TaxType,
                SourceDocumentType = original.SourceDocumentType,
                SourceDocumentId = original.SourceDocumentId,
                SourceDocumentNumberSnapshot = "REV-" + original.SourceDocumentNumberSnapshot,
                SourceDocumentDateSnapshot = DateTime.Today,
                PostingDate = DateTime.Today,
                AccountingPeriodId = original.AccountingPeriodId,
                TaxPeriodKey = original.TaxPeriodKey,
                IsCreditOrDebitAdjustment = true,
                OriginalSourceDocumentId = original.SourceDocumentId,
                PartyType = original.PartyType,
                PartyId = original.PartyId,
                PartyNameSnapshot = original.PartyNameSnapshot,
                PartyGSTINSnapshot = original.PartyGSTINSnapshot,
                PartyPANSnapshot = original.PartyPANSnapshot,
                PartyRegistrationTypeSnapshot = original.PartyRegistrationTypeSnapshot,
                IsPartyGSTRegistered = original.IsPartyGSTRegistered,
                SupplyType = original.SupplyType,
                FromStateCode = original.FromStateCode,
                ToStateCode = original.ToStateCode,
                PlaceOfSupplyMode = original.PlaceOfSupplyMode,
                IsReverseCharge = original.IsReverseCharge,
                TaxableValueTotal = -original.TaxableValueTotal,
                TaxAmountTotal = -original.TaxAmountTotal,
                GrossDocumentValueSnapshot = original.GrossDocumentValueSnapshot.HasValue
                                                    ? -original.GrossDocumentValueSnapshot : null,
                ITCEligibleTaxAmount = -original.ITCEligibleTaxAmount,
                ITCIneligibleTaxAmount = -original.ITCIneligibleTaxAmount,
                RCMLiabilityTaxAmount = -original.RCMLiabilityTaxAmount,
                WithholdingTaxAmount = -original.WithholdingTaxAmount,
                CollectionTaxAmount = -original.CollectionTaxAmount,
                AppliedMappingIdSnapshot = original.AppliedMappingIdSnapshot,
                AppliedMappingCodeSnapshot = original.AppliedMappingCodeSnapshot,
                AppliedMappingVersionSnapshot = original.AppliedMappingVersionSnapshot,
                RateResolutionBasisSnapshot = original.RateResolutionBasisSnapshot,
                RateVersionResolutionModeSnapshot = original.RateVersionResolutionModeSnapshot,
                CalculationEngineVersion = original.CalculationEngineVersion,
                IsIncludedInReturn = false,
                ReturnInclusionStatus = ReturnInclusionStatus.Pending,
                ReconciliationStatus = ReconciliationStatus1.NotReconciled,
                ReversalTaxTransactionId = original.Id,
                ReversalReason = reason,
                TaxTransactionStatus = TaxTransactionStatus.Draft,
                Lines = original.Lines.Select(l => new TaxTransactionLineModel
                {
                    TenantId = l.TenantId,
                    CompanyId = l.CompanyId,
                    TaxTransactionId = Guid.Empty,
                    LineNumber = l.LineNumber,
                    SourceDocumentTypeSnapshot = l.SourceDocumentTypeSnapshot,
                    SourceDocumentIdSnapshot = l.SourceDocumentIdSnapshot,
                    SourceLineId = l.SourceLineId,
                    SourceLineNumberSnapshot = l.SourceLineNumberSnapshot,
                    SourceLineDescriptionSnapshot = "REVERSAL: " + l.SourceLineDescriptionSnapshot,
                    SourceLineAmountSnapshot = l.SourceLineAmountSnapshot.HasValue
                                                        ? -l.SourceLineAmountSnapshot : null,
                    TaxCodeId = l.TaxCodeId,
                    TaxCodeSnapshot = l.TaxCodeSnapshot,
                    TaxNameSnapshot = l.TaxNameSnapshot,
                    TaxTypeSnapshot = l.TaxTypeSnapshot,
                    GSTComponentTypeSnapshot = l.GSTComponentTypeSnapshot,
                    TaxDirectionSnapshot = l.TaxDirectionSnapshot,
                    AppliedMappingIdSnapshot = l.AppliedMappingIdSnapshot,
                    AppliedMappingCodeSnapshot = l.AppliedMappingCodeSnapshot,
                    AppliedMappingLineIdSnapshot = l.AppliedMappingLineIdSnapshot,
                    TaxRateVersionId = l.TaxRateVersionId,
                    RateVersionNumberSnapshot = l.RateVersionNumberSnapshot,
                    RateTypeSnapshot = l.RateTypeSnapshot,
                    RatePercentSnapshot = l.RatePercentSnapshot,
                    FixedAmountSnapshot = l.FixedAmountSnapshot,
                    RateResolutionDateSnapshot = l.RateResolutionDateSnapshot,
                    RateResolutionBasisSnapshot = l.RateResolutionBasisSnapshot,
                    TaxableBaseAmount = -l.TaxableBaseAmount,
                    AssessableValueAmount = l.AssessableValueAmount.HasValue
                                                        ? -l.AssessableValueAmount : null,
                    TaxAmount = -l.TaxAmount,
                    GrossLineValueSnapshot = l.GrossLineValueSnapshot.HasValue
                                                        ? -l.GrossLineValueSnapshot : null,
                    InclusiveExclusiveMode = l.InclusiveExclusiveMode,
                    RoundingDifferenceAmount = 0,
                    CalculationFormulaSnapshot = $"-({l.CalculationFormulaSnapshot})",
                    SupplyTypeSnapshot = l.SupplyTypeSnapshot,
                    FromStateCodeSnapshot = l.FromStateCodeSnapshot,
                    ToStateCodeSnapshot = l.ToStateCodeSnapshot,
                    IsReverseChargeLine = l.IsReverseChargeLine,
                    ITCEligibilityStatus = l.ITCEligibilityStatus,
                    GSTReturnTagSnapshot = l.GSTReturnTagSnapshot,
                    ExemptionClassification = l.ExemptionClassification,
                    SectionCodeSnapshot = l.SectionCodeSnapshot,
                    ThresholdAppliedFlag = l.ThresholdAppliedFlag,
                    ThresholdAmountSnapshot = l.ThresholdAmountSnapshot,
                    IsPanMissingAlternateRateApplied = l.IsPanMissingAlternateRateApplied,
                    BaseForWithholdingAmount = l.BaseForWithholdingAmount.HasValue
                                                        ? -l.BaseForWithholdingAmount : null,
                    LineStatus = TaxTransactionLineStatus.Reversed,
                    IsIncludedInReturn = false,
                    SettlementAppliedAmount = 0,
                    ReversalOfTaxTransactionLineId = l.Id,
                    ReversalReason = reason
                }).ToList()
            };

            Create(reversal);
            Post(reversal.Id, "system-reversal");

            original.ReversalTaxTransactionId = reversal.Id;
        }

        public (int included, int skipped) BulkIncludeInReturn(
            List<Guid> ids, Guid? gstReturnRunId = null)
        {
            int included = 0, skipped = 0;

            foreach (var id in ids)
            {
                try
                {
                    IncludeInReturn(id, gstReturnRunId);
                    included++;
                }
                catch
                {
                    skipped++;
                }
            }

            return (included, skipped);
        }

        // ─── SUMMARY ───────────────────────────────────────────────────────

        public TaxTransactionSummary GetSummary(
            Guid? companyId = null,
            string? periodKey = null,
            string? taxType = null)
        {
            var q = _transactions.AsEnumerable();

            if (companyId.HasValue)
                q = q.Where(t => t.CompanyId == companyId.Value);

            if (!string.IsNullOrWhiteSpace(periodKey))
                q = q.Where(t => t.TaxPeriodKey == periodKey);

            if (!string.IsNullOrWhiteSpace(taxType))
                q = q.Where(t => t.TaxType == taxType);

            var list = q.ToList();

            return new TaxTransactionSummary
            {
                TotalCount = list.Count,
                PostedCount = list.Count(t => t.TaxTransactionStatus == TaxTransactionStatus.Posted),
                IncludedCount = list.Count(t => t.TaxTransactionStatus == TaxTransactionStatus.IncludedInReturn),
                ReversedCount = list.Count(t => t.TaxTransactionStatus == TaxTransactionStatus.Reversed),
                ExcludedCount = list.Count(t => t.ReturnInclusionStatus == ReturnInclusionStatus.Excluded),
                TotalTaxableValue = list.Sum(t => t.TaxableValueTotal),
                TotalTaxAmount = list.Sum(t => t.TaxAmountTotal),
                TotalITCEligible = list.Sum(t => t.ITCEligibleTaxAmount),
                TotalWithholding = list.Sum(t => t.WithholdingTaxAmount)
            };
        }

        private static void ValidateCreate(TaxTransactionModel tx)
        {
            if (tx.SourceDocumentId == Guid.Empty)
                throw new Exception("Source Document reference is required");

            if (string.IsNullOrWhiteSpace(tx.SourceDocumentNumberSnapshot))
                throw new Exception("Source Document Number snapshot is required");

            if (tx.PartyId == Guid.Empty)
                throw new Exception("Party is required");

            if (tx.PostingDate == default)
                throw new Exception("Posting Date is required");
        }

        private static void ValidateForPosting(TaxTransactionModel tx)
        {
            if (tx.TaxableValueTotal < 0 && !tx.IsCreditOrDebitAdjustment)
                throw new Exception("Taxable value cannot be negative for non-adjustment transactions");

            if (string.IsNullOrWhiteSpace(tx.TaxType))
                throw new Exception("Tax Type is required");

            if (tx.TaxType == "GST" && string.IsNullOrWhiteSpace(tx.SupplyType))
                throw new Exception("Supply Type is required for GST transactions");

            if (tx.TaxType == "GST" && string.IsNullOrWhiteSpace(tx.FromStateCode))
                throw new Exception("From State Code is required for GST transactions");

            if (!tx.Lines.Any())
                throw new Exception("Tax Transaction must have at least one line");

            if (tx.SupplyType == "IntraState")
            {
                var igstLine = tx.Lines.FirstOrDefault(l => l.GSTComponentTypeSnapshot == "IGST");
                if (igstLine != null)
                    throw new Exception("Intra-state transaction cannot contain an IGST line");
            }

            if (tx.SupplyType == "InterState")
            {
                var hasCgst = tx.Lines.Any(l => l.GSTComponentTypeSnapshot == "CGST");
                var hasSgst = tx.Lines.Any(l => l.GSTComponentTypeSnapshot == "SGST");
                if (hasCgst && hasSgst)
                    throw new Exception("Inter-state transaction cannot contain both CGST and SGST lines");
            }
        }

        private static void RecalculateTotals(TaxTransactionModel tx)
        {
            tx.TaxableValueTotal = tx.Lines.Sum(l => l.TaxableBaseAmount);
            tx.TaxAmountTotal = tx.Lines.Sum(l => l.TaxAmount);
            tx.ITCEligibleTaxAmount = tx.Lines
                .Where(l => l.ITCEligibilityStatus == "Eligible")
                .Sum(l => l.TaxAmount);
            tx.ITCIneligibleTaxAmount = tx.Lines
                .Where(l => l.ITCEligibilityStatus == "Ineligible")
                .Sum(l => l.TaxAmount);
            tx.RCMLiabilityTaxAmount = tx.Lines
                .Where(l => l.IsReverseChargeLine && l.TaxDirectionSnapshot == "Output")
                .Sum(l => l.TaxAmount);
            tx.WithholdingTaxAmount = tx.Lines
                .Where(l => l.TaxDirectionSnapshot == "WithholdingPayable")
                .Sum(l => l.TaxAmount);
            tx.CollectionTaxAmount = tx.Lines
                .Where(l => l.TaxTypeSnapshot == "TCS")
                .Sum(l => l.TaxAmount);
        }


        public void Delete(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.TaxTransactionStatus != TaxTransactionStatus.Draft)
                throw new Exception("Only Draft transactions can be deleted");

            _lines.RemoveAll(l => l.TaxTransactionId == id);
            _transactions.Remove(tx);
        }
    }

    // ─── Status Constants ──────────────────────────────────────────────────

    public static class TaxTransactionStatus
    {
        public const string Draft = "Draft";
        public const string Posted = "Posted";
        public const string IncludedInReturn = "IncludedInReturn";
        public const string Excluded = "Excluded";
        public const string PartiallySettled = "PartiallySettled";
        public const string Settled = "Settled";
        public const string Reversed = "Reversed";
        public const string Cancelled = "Cancelled";
    }

    public static class TaxTransactionLineStatus
    {
        public const string Posted = "Posted";
        public const string Excluded = "Excluded";
        public const string Reversed = "Reversed";
        public const string Settled = "Settled";
        public const string PartiallySettled = "PartiallySettled";
    }

    public static class ReturnInclusionStatus
    {
        public const string NotApplicable = "NotApplicable";
        public const string Pending = "Pending";
        public const string Included = "Included";
        public const string Excluded = "Excluded";
        public const string Amended = "Amended";
    }

    public static class ReconciliationStatus1
    {
        public const string NotReconciled = "NotReconciled";
        public const string Matched = "Matched";
        public const string PartiallyMatched = "PartiallyMatched";
        public const string Mismatch = "Mismatch";
    }

    public class TaxTransactionSummary
    {
        public int TotalCount { get; set; }
        public int PostedCount { get; set; }
        public int IncludedCount { get; set; }
        public int ReversedCount { get; set; }
        public int ExcludedCount { get; set; }
        public decimal TotalTaxableValue { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalITCEligible { get; set; }
        public decimal TotalWithholding { get; set; }
    }
}
