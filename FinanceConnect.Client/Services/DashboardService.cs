using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class DashboardService
    {
        private readonly DashboardViewModel _dashboardData = new();

        public event Action? OnChange;

        public Task<DashboardViewModel> GetDashboardDataAsync()
        {
            return Task.FromResult(_dashboardData);
        }

        public Task SaveDashboardDataAsync(Guid? companyId, Guid? branchId)
        {
            _dashboardData.SelectedCompanyId = companyId;
            _dashboardData.SelectedBranchId = branchId;

            OnChange?.Invoke();   // notify listeners
            return Task.CompletedTask;
        }
    }

}
