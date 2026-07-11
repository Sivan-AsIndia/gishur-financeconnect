using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TaxCategoryMappingViewModel;

namespace FinanceConnect.Client.Data
{
    public class TaxCategoryMappingSeedData
    {
        // ── GUIDs ─────────────────────────────────────────────────────────────
        private static readonly Guid ID1 = Guid.Parse("BB000000-0000-0000-0000-000000000001");
        private static readonly Guid ID2 = Guid.Parse("BB000000-0000-0000-0000-000000000002");
        private static readonly Guid ID3 = Guid.Parse("BB000000-0000-0000-0000-000000000003");
        private static readonly Guid ID4 = Guid.Parse("BB000000-0000-0000-0000-000000000004");
        private static readonly Guid ID5 = Guid.Parse("BB000000-0000-0000-0000-000000000005");

        public List<TaxCategoryMappingListDto> Store { get; } = new()
        {
            new TaxCategoryMappingListDto
            {
                TaxCategoryMappingId  = ID1,
                MappingCode           = "MAP-GST-INTRA-18",
                MappingName           = "Intra-State GST 18% – Standard Goods",
                TaxTypeScope          = TaxTypeScope.GST,
                TransactionContext    = TransactionContext.AR_SalesInvoice,
                MappingStatus         = MappingStatus.Active,
                Priority              = 10,
                EffectiveFrom         = new DateTime(2024, 4, 1),
                EffectiveTo           = null,
                SupplyType            = SupplyType.IntraState,
                IsExemptOrNilOrNonGST = ExemptType.None,
                IsLockedForChanges    = false,
                LineCount             = 2,
                CreatedAt             = new DateTime(2024, 4, 1),
            },
            new TaxCategoryMappingListDto
            {
                TaxCategoryMappingId  = ID2,
                MappingCode           = "MAP-GST-INTER-18",
                MappingName           = "Inter-State GST 18% – Standard Goods",
                TaxTypeScope          = TaxTypeScope.GST,
                TransactionContext    = TransactionContext.AR_SalesInvoice,
                MappingStatus         = MappingStatus.Active,
                Priority              = 10,
                EffectiveFrom         = new DateTime(2024, 4, 1),
                EffectiveTo           = null,
                SupplyType            = SupplyType.InterState,
                IsExemptOrNilOrNonGST = ExemptType.None,
                IsLockedForChanges    = false,
                LineCount             = 1,
                CreatedAt             = new DateTime(2024, 4, 1),
            },
            new TaxCategoryMappingListDto
            {
                TaxCategoryMappingId  = ID3,
                MappingCode           = "MAP-TDS-194C-PAY",
                MappingName           = "TDS Contractor Payment – 194C Withholding",
                TaxTypeScope          = TaxTypeScope.TDS,
                TransactionContext    = TransactionContext.AP_VendorPayment,
                MappingStatus         = MappingStatus.Active,
                Priority              = 10,
                EffectiveFrom         = new DateTime(2024, 4, 1),
                EffectiveTo           = null,
                SupplyType            = null,
                IsExemptOrNilOrNonGST = ExemptType.None,
                IsLockedForChanges    = false,
                LineCount             = 1,
                CreatedAt             = new DateTime(2024, 4, 1),
            },
            new TaxCategoryMappingListDto
            {
                TaxCategoryMappingId  = ID4,
                MappingCode           = "MAP-GST-EXEMPT",
                MappingName           = "Exempt Supply – No Tax",
                TaxTypeScope          = TaxTypeScope.GST,
                TransactionContext    = TransactionContext.AR_SalesInvoice,
                MappingStatus         = MappingStatus.Active,
                Priority              = 5,
                EffectiveFrom         = new DateTime(2024, 4, 1),
                EffectiveTo           = null,
                SupplyType            = null,
                IsExemptOrNilOrNonGST = ExemptType.Exempt,
                IsLockedForChanges    = false,
                LineCount             = 0,
                CreatedAt             = new DateTime(2024, 4, 1),
            },
            new TaxCategoryMappingListDto
            {
                TaxCategoryMappingId  = ID5,
                MappingCode           = "MAP-GST-AP-RCM",
                MappingName           = "Purchase Bill – Reverse Charge (RCM)",
                TaxTypeScope          = TaxTypeScope.GST,
                TransactionContext    = TransactionContext.AP_PurchaseBill,
                MappingStatus         = MappingStatus.Active,
                Priority              = 20,
                EffectiveFrom         = new DateTime(2024, 4, 1),
                EffectiveTo           = null,
                SupplyType            = SupplyType.IntraState,
                IsExemptOrNilOrNonGST = ExemptType.None,
                IsLockedForChanges    = true,
                LineCount             = 2,
                CreatedAt             = new DateTime(2024, 4, 1),
                UpdatedAt             = new DateTime(2024, 6, 15),
            },
        };

        public Dictionary<Guid, List<TaxCategoryMappingLineModel>> Lines { get; } = new()
        {
            [ID1] = new List<TaxCategoryMappingLineModel>
            {
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000001-0000-0000-0000-000000000001"),
                    TaxCategoryMappingId   = ID1,
                    LineNumber             = 10,
                    TaxCodeId              = Guid.Parse("cc000001-0000-0000-0000-000000000001"),
                    TaxCodeCode            = "GST_CGST",
                    TaxCodeName            = "CGST – Central GST",
                    TaxCodeComponent       = "CGST",
                    TaxCodeDirection       = "Output",
                    ApplyMode              = "AddOn",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Inherit",
                    RCMBehavior            = "Normal",
                    IsLineActive           = true,
                    LineNotes              = "Central GST 9%",
                },
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000001-0000-0000-0000-000000000002"),
                    TaxCategoryMappingId   = ID1,
                    LineNumber             = 20,
                    TaxCodeId              = Guid.Parse("cc000002-0000-0000-0000-000000000002"),
                    TaxCodeCode            = "GST_SGST",
                    TaxCodeName            = "SGST – State GST",
                    TaxCodeComponent       = "SGST",
                    TaxCodeDirection       = "Output",
                    ApplyMode              = "AddOn",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Inherit",
                    RCMBehavior            = "Normal",
                    IsLineActive           = true,
                    LineNotes              = "State GST 9%",
                },
            },

            [ID2] = new List<TaxCategoryMappingLineModel>
            {
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000002-0000-0000-0000-000000000001"),
                    TaxCategoryMappingId   = ID2,
                    LineNumber             = 10,
                    TaxCodeId              = Guid.Parse("cc000003-0000-0000-0000-000000000003"),
                    TaxCodeCode            = "GST_IGST",
                    TaxCodeName            = "IGST – Integrated GST",
                    TaxCodeComponent       = "IGST",
                    TaxCodeDirection       = "Output",
                    ApplyMode              = "AddOn",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Inherit",
                    RCMBehavior            = "Normal",
                    IsLineActive           = true,
                    LineNotes              = "Integrated GST 18%",
                },
            },

            [ID3] = new List<TaxCategoryMappingLineModel>
            {
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000003-0000-0000-0000-000000000001"),
                    TaxCategoryMappingId   = ID3,
                    LineNumber             = 10,
                    TaxCodeId              = Guid.Parse("cc000006-0000-0000-0000-000000000006"),
                    TaxCodeCode            = "TDS_194C",
                    TaxCodeName            = "TDS – Section 194C (Contractors)",
                    TaxCodeComponent       = "TDS",
                    TaxCodeDirection       = "Withholding",
                    ApplyMode              = "Withholding",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Ineligible",
                    RCMBehavior            = "Normal",
                    IsLineActive           = true,
                    LineNotes              = "TDS deducted at source – 194C",
                },
            },

            [ID4] = new List<TaxCategoryMappingLineModel>(),

            [ID5] = new List<TaxCategoryMappingLineModel>
            {
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000005-0000-0000-0000-000000000001"),
                    TaxCategoryMappingId   = ID5,
                    LineNumber             = 10,
                    TaxCodeId              = Guid.Parse("cc000001-0000-0000-0000-000000000001"),
                    TaxCodeCode            = "GST_CGST",
                    TaxCodeName            = "CGST – Central GST",
                    TaxCodeComponent       = "CGST",
                    TaxCodeDirection       = "Input",
                    ApplyMode              = "AddOn",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Eligible",
                    RCMBehavior            = "RCM_OutputPlusInputCredit",
                    IsLineActive           = true,
                    LineNotes              = "RCM – CGST output liability + ITC",
                },
                new TaxCategoryMappingLineModel
                {
                    Id                     = Guid.Parse("AA000005-0000-0000-0000-000000000002"),
                    TaxCategoryMappingId   = ID5,
                    LineNumber             = 20,
                    TaxCodeId              = Guid.Parse("cc000002-0000-0000-0000-000000000002"),
                    TaxCodeCode            = "GST_SGST",
                    TaxCodeName            = "SGST – State GST",
                    TaxCodeComponent       = "SGST",
                    TaxCodeDirection       = "Input",
                    ApplyMode              = "AddOn",
                    RateResolutionMode     = "FromTaxRateVersionByDate",
                    RateEffectiveDateBasis = "PostingDate",
                    ITCEligibilityOverride = "Eligible",
                    RCMBehavior            = "RCM_OutputPlusInputCredit",
                    IsLineActive           = true,
                    LineNotes              = "RCM – SGST output liability + ITC",
                },
            },
        };
        public List<TaxCategoryMappingLineModel> GetLines(Guid mappingId)
            => Lines.TryGetValue(mappingId, out var lines) ? lines : new List<TaxCategoryMappingLineModel>();
    }
}
