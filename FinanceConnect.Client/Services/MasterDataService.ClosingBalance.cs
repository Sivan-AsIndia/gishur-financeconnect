using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Closing Balance Methods (Read-only - System Generated)
        public List<ClosingBalanceModel> GetAllClosingBalances() => _closingBalances.ToList();

        public ClosingBalanceModel? GetClosingBalanceById(Guid id) => _closingBalances.FirstOrDefault(c => c.Id == id);

        public List<ClosingBalanceModel> GetClosingBalancesByCompany(Guid companyId) =>
            _closingBalances.Where(c => c.CompanyId == companyId).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByBranch(Guid branchId) =>
            _closingBalances.Where(c => c.BranchId == branchId).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByPeriod(Guid accountingPeriodId) =>
            _closingBalances.Where(c => c.AccountingPeriodId == accountingPeriodId).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByAccount(Guid accountId) =>
            _closingBalances.Where(c => c.AccountId == accountId).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByCloseRun(Guid closeRunId) =>
            _closingBalances.Where(c => c.CloseRunId == closeRunId).ToList();

        // Get unique values for filter dropdowns
        public List<(Guid Id, string Code, string Name)> GetCBCompanies() =>
            _closingBalances.Select(c => (c.CompanyId, c.CompanyCode ?? "", c.CompanyName ?? "")).Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetCBBranches() =>
            _closingBalances.Select(c => (c.BranchId, c.BranchCode ?? "", c.BranchName ?? "")).Distinct().ToList();

        public List<(Guid Id, string Name)> GetCBPeriods() =>
            _closingBalances.Select(c => (c.AccountingPeriodId, c.AccountingPeriodName ?? "")).Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetCBAccounts() =>
            _closingBalances.Select(c => (c.AccountId, c.AccountCode ?? "", c.AccountName ?? "")).Distinct().ToList();

        // Summary calculations
        public (decimal TotalDebit, decimal TotalCredit) GetCBTotals(List<ClosingBalanceModel> balances)
        {
            var totalDebit = balances.Sum(b => b.ClosingDebit);
            var totalCredit = balances.Sum(b => b.ClosingCredit);
            return (totalDebit, totalCredit);
        }
    }
}
