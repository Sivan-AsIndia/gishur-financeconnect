using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        public List<GeneralLedgerEntryModel> GetAllGeneralLedgerEntries() =>
            _generalLedgerEntries.Where(g => !g.IsDeleted).OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public GeneralLedgerEntryModel? GetGeneralLedgerEntryById(Guid id) =>
            _generalLedgerEntries.FirstOrDefault(g => g.Id == id && !g.IsDeleted);

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesByCompany(Guid companyId) =>
            _generalLedgerEntries.Where(g => g.CompanyId == companyId && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesByBranch(Guid branchId) =>
            _generalLedgerEntries.Where(g => g.BranchId == branchId && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesByAccount(Guid accountId) =>
            _generalLedgerEntries.Where(g => g.AccountId == accountId && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesByPeriod(Guid accountingPeriodId) =>
            _generalLedgerEntries.Where(g => g.AccountingPeriodId == accountingPeriodId && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesByDateRange(DateTime fromDate, DateTime toDate) =>
            _generalLedgerEntries.Where(g => g.PostingDate >= fromDate && g.PostingDate <= toDate && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesBySourceDocument(Guid sourceDocumentId) =>
            _generalLedgerEntries.Where(g => g.SourceDocumentId == sourceDocumentId && !g.IsDeleted)
                .OrderBy(g => g.PostingSequenceNumber).ToList();

        public List<GeneralLedgerEntryModel> GetGeneralLedgerEntriesBySourceType(string sourceType) =>
            _generalLedgerEntries.Where(g => g.SourceType == sourceType && !g.IsDeleted)
                .OrderByDescending(g => g.PostingDate).ThenByDescending(g => g.PostingSequenceNumber).ToList();

        // Get unique values for filter dropdowns
        public List<(Guid Id, string Code, string Name)> GetGLCompanies() =>
            _generalLedgerEntries.Where(g => !g.IsDeleted)
                .Select(g => (g.CompanyId, g.CompanyCode ?? "", g.CompanyName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetGLBranches() =>
            _generalLedgerEntries.Where(g => !g.IsDeleted)
                .Select(g => (g.BranchId, g.BranchCode ?? "", g.BranchName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetGLAccounts() =>
            _generalLedgerEntries.Where(g => !g.IsDeleted)
                .Select(g => (g.AccountId, g.AccountCode ?? "", g.AccountName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetGLPeriods() =>
            _generalLedgerEntries.Where(g => !g.IsDeleted)
                .Select(g => (g.AccountingPeriodId, g.AccountingPeriodCode ?? "", g.AccountingPeriodName ?? ""))
                .Distinct().ToList();

        // Summary calculations
        public (decimal TotalDebit, decimal TotalCredit) GetGLTotals(List<GeneralLedgerEntryModel> entries)
        {
            var totalDebit = entries.Sum(e => e.DebitAmount);
            var totalCredit = entries.Sum(e => e.CreditAmount);
            return (totalDebit, totalCredit);
        }
    }
}
