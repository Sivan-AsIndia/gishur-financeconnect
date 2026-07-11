namespace FinanceConnect.Client.Services
{
    public class SettingsService
    {

        public Guid? SelectedCompanyId { get; private set; }
        public Guid? SelectedBranchId { get; private set; }
        public string? SelectedBranchName { get; private set; }

        public event Action? OnChange;

        public void SetWorkspace(Guid? companyId, Guid? branchId, string? branchName)
        {
            SelectedCompanyId = companyId;
            SelectedBranchId = branchId;
            SelectedBranchName = branchName;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
