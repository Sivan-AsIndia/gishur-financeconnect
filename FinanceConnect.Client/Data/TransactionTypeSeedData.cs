using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class TransactionTypeSeedData
    {
        public static List<TransactionTypeModel> SeedForCompanies(
            List<CompanyModel> companies,
            List<PostingProfileModel> postingProfiles,
            List<DocumentNumberSeriesModel> documentNumbers)
        {
            var list = new List<TransactionTypeModel>();

            foreach (var company in companies)
            {
                var companyProfiles = postingProfiles
                    .Where(p => p.CompanyId == company.Id && p.IsActive)
                    .ToList();

                var apProfile = companyProfiles.FirstOrDefault();

                var documentNumber = documentNumbers.FirstOrDefault(d =>
                    d.CompanyId == company.Id &&
                    d.AppliesToEntityType == AppliesToEntityType.FinancialTransaction);

                var suffix = string.IsNullOrWhiteSpace(company.CompanyCode)
                    ? company.Id.ToString("N")[..6]
                    : company.CompanyCode.ToUpper();

                var code = $"AP_VENDOR_BILL_{suffix}";
                var name = $"Vendor Bill - {company.CompanyCode ?? company.LegalName}";

                list.Add(new TransactionTypeModel
                {
                    TransactionTypeId = Guid.NewGuid(),
                    TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CompanyId = company.Id,
                    CompanyName = company.LegalName,

                    Code = code,
                    Name = name,

                    SourceCategory = SourceCategory.AP,
                    TransactionNature = TransactionNature.Accrual,

                    IsPostable = true,
                    AllowAutoPost = false,
                    RequiresApproval = true,
                    ApprovalWorkflowKey = $"FIN_TXN_{company.CompanyCode}_AP_VB_WORKFLOW",

                    AllowManualEntry = false,
                    AllowDraftEdit = true,
                    AllowDraftCancel = true,
                    AllowReversal = true,

                    AllowForeignCurrency = true,
                    AllowNegativeLines = false,
                    AmountPrecisionPolicy = 2,

                    DocumentNoAssignmentTiming = DocumentNoAssignmentTimings.OnApproval,
                    DocumentNumberSeriesId = documentNumber?.DocumentNumberSeriesId,

                    // 🔹 Resolved profile
                    DefaultPostingProfileId = apProfile?.PostingProfileId,

                    IsSystemDefined = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            return list;
        }

    }
}
