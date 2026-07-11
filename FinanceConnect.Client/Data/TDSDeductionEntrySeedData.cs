using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TDSDeductionEntryViewModel;

namespace FinanceConnect.Client.Data
{
    public class TDSDeductionEntrySeedData
    {
        public List<TDSDeductionEntryListDto> Store { get; } = new()
        {
            new TDSDeductionEntryListDto
            {
                TDSDeductionEntryId             = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                DeductionNumber                 = "TDS-000001",
                Status                          = DeductionStatus.Posted,
                DeductionDate                   = new DateTime(2025, 4, 5),
                PostingDate                     = new DateTime(2025, 4, 5),
                SourceDocumentType              = SourceDocumentType.VendorPayment,
                SourceDocumentNumberSnapshot    = "VP-00045",
                VendorCodeSnapshot              = "VND-001",
                VendorNameSnapshot              = "Infosys Limited",
                VendorPANSnapshot               = "AAACI1681G",
                VendorResidencySnapshot         = VendorResidency.Resident,
                SectionCodeSnapshot             = "194C",
                TaxCodeSnapshot                 = "TDS-194C",
                RatePercentApplied              = 1.00m,
                IsAlternatePanRateApplied       = false,
                DeductionBaseAmount             = 90000m,
                DeductionAmount                 = 900m,
                SettledAmount                   = 0m,
                SettlementStatus                = SettlementStatus.NotSettled,
                ThresholdEvaluationModeSnapshot = ThresholdEvaluationMode.CumulativeByVendorInFinancialYear,
                ThresholdTriggeredFlag          = true,
                PostedOn                        = new DateTime(2025, 4, 5),
                CreatedAt                       = new DateTime(2025, 4, 5)
            },
            new TDSDeductionEntryListDto
            {
                TDSDeductionEntryId             = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                DeductionNumber                 = "TDS-000002",
                Status                          = DeductionStatus.PartiallySettled,
                DeductionDate                   = new DateTime(2025, 4, 10),
                PostingDate                     = new DateTime(2025, 4, 10),
                SourceDocumentType              = SourceDocumentType.VendorBill,
                SourceDocumentNumberSnapshot    = "VB-00122",
                VendorCodeSnapshot              = "VND-002",
                VendorNameSnapshot              = "TCS Limited",
                VendorPANSnapshot               = "AAACR4849P",
                VendorResidencySnapshot         = VendorResidency.Resident,
                SectionCodeSnapshot             = "194J",
                TaxCodeSnapshot                 = "TDS-194J",
                RatePercentApplied              = 10.00m,
                IsAlternatePanRateApplied       = false,
                DeductionBaseAmount             = 200000m,
                DeductionAmount                 = 20000m,
                SettledAmount                   = 10000m,
                SettlementStatus                = SettlementStatus.PartiallySettled,
                ThresholdEvaluationModeSnapshot = ThresholdEvaluationMode.NoThreshold,
                ThresholdTriggeredFlag          = false,
                PostedOn                        = new DateTime(2025, 4, 10),
                CreatedAt                       = new DateTime(2025, 4, 10)
            },
            new TDSDeductionEntryListDto
            {
                TDSDeductionEntryId             = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                DeductionNumber                 = "TDS-000003",
                Status                          = DeductionStatus.Posted,
                DeductionDate                   = new DateTime(2025, 4, 18),
                PostingDate                     = new DateTime(2025, 4, 18),
                SourceDocumentType              = SourceDocumentType.VendorPayment,
                SourceDocumentNumberSnapshot    = "VP-00067",
                VendorCodeSnapshot              = "VND-003",
                VendorNameSnapshot              = "Wipro Technologies",
                VendorPANSnapshot               = null,
                VendorResidencySnapshot         = VendorResidency.Resident,
                SectionCodeSnapshot             = "194C",
                TaxCodeSnapshot                 = "TDS-194C",
                RatePercentApplied              = 20.00m,
                IsAlternatePanRateApplied       = true,
                DeductionBaseAmount             = 50000m,
                DeductionAmount                 = 10000m,
                SettledAmount                   = 0m,
                SettlementStatus                = SettlementStatus.NotSettled,
                ThresholdEvaluationModeSnapshot = ThresholdEvaluationMode.PerTransaction,
                ThresholdTriggeredFlag          = true,
                PostedOn                        = new DateTime(2025, 4, 18),
                CreatedAt                       = new DateTime(2025, 4, 18)
            },
            new TDSDeductionEntryListDto
            {
                TDSDeductionEntryId             = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                DeductionNumber                 = "TDS-000004",
                Status                          = DeductionStatus.Settled,
                DeductionDate                   = new DateTime(2025, 3, 15),
                PostingDate                     = new DateTime(2025, 3, 15),
                SourceDocumentType              = SourceDocumentType.VendorPayment,
                SourceDocumentNumberSnapshot    = "VP-00038",
                VendorCodeSnapshot              = "VND-004",
                VendorNameSnapshot              = "HCL Technologies",
                VendorPANSnapshot               = "AAACP4959J",
                VendorResidencySnapshot         = VendorResidency.Resident,
                SectionCodeSnapshot             = "194J",
                TaxCodeSnapshot                 = "TDS-194J",
                RatePercentApplied              = 10.00m,
                IsAlternatePanRateApplied       = false,
                DeductionBaseAmount             = 150000m,
                DeductionAmount                 = 15000m,
                SettledAmount                   = 15000m,
                SettlementStatus                = SettlementStatus.FullySettled,
                ThresholdEvaluationModeSnapshot = ThresholdEvaluationMode.NoThreshold,
                ThresholdTriggeredFlag          = false,
                PostedOn                        = new DateTime(2025, 3, 15),
                CreatedAt                       = new DateTime(2025, 3, 15)
            },
        };
    }
}
