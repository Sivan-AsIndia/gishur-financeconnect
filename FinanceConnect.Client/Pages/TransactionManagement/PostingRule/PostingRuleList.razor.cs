using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.TransactionManagement.PostingRule
{
    public partial class PostingRuleList
    {
        [Parameter] public Guid ProfileId { get; set; }

        // DEPENDENCIES
        [Inject] PostingRuleService RuleService { get; set; } = default!;
        [Inject] PostingProfileService ProfileService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        private readonly PostingRuleSeed _statusSeed = new();
        // STATE
        PostingProfileModel? SelectedProfile;
        PostingRuleModel? SelectedRule;

        List<PostingRuleModel> Rules = new();
        List<PostingRuleModel> FilteredRules = new();
        private int VisibleColumnCount;
        // FILTERS
        string searchText = "";
        string selectedStatus = "";
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        bool canDeactivate = true;
        bool canActivate = true;
        private bool isInitialized = false;
        private bool isLoading = false;
        // PAGINATION
        int TotalPages =>
            FilteredRules.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredRules.Count / PageSize);

        List<PostingRuleModel> PagedRules =>
            FilteredRules
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - (PageWindowSize / 2));
                int end = start + PageWindowSize - 1;

                if (end > TotalPages)
                {
                    end = TotalPages;
                    start = end - PageWindowSize + 1;
                }

                return Enumerable.Range(start, end - start + 1);
            }
        }

        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        // LIFECYCLE
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
            await JS.InvokeVoidAsync("initTooltips",true);
        }

        protected override void OnInitialized()
        {
            SelectedProfile = ProfileService.GetById(ProfileId);

            if (SelectedProfile == null)
            {
                ToastService.ShowError("Posting Profile not found");
                Nav.NavigateTo("/posting-profiles");
                return;
            }

            // OPTIONAL DEV SEED (remove in prod)
            //if (!RuleService.GetByProfile(ProfileId).Any() && SelectedProfile.CreatedBy == "seed")
            //{
            //    RuleService.Seed(
            //        profileId: ProfileId,
            //        companyId: SelectedProfile.CompanyId,
            //        tenantId: SelectedProfile.TenantId
            //    );
            //}

            LoadRules();
        }

        // LOAD & REFRESH
        void LoadRules()
        {
            Rules = RuleService.GetByProfile(ProfileId);
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
        }

        // FILTERING
        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void ApplyFilters()
        {
            IEnumerable<PostingRuleModel> query = Rules;

            // Search: Rule Code / Name
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(r =>
                    r.RuleCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    r.RuleName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Status Filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                bool active = selectedStatus == "Active";
                query = query.Where(r => r.IsActive == active);
            }

            // Deterministic order
            FilteredRules = query
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.RuleCode)
                .ToList();

            CurrentPage = 1;
        }

        // PAGINATION
        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
        }

        // ACTIONS
        void ViewRule(PostingRuleModel rule)
        {
            SelectedRule = rule;
        }
        void GoToRuleDetails(PostingRuleModel rule)
        {
            SelectedRule = rule;
            Nav.NavigateTo($"/posting-profiles/rules/{rule.PostingRuleId}/view");
        }

        void SelectRule(PostingRuleModel rule)
        {
            SelectedRule = rule;
        }

        void AddRule()
        {
            Nav.NavigateTo($"/posting-profiles/{ProfileId}/rules/create");
        }

        void EditRule(PostingRuleModel rule)
        {
            Nav.NavigateTo($"/posting-profiles/{ProfileId}/rules/{rule.PostingRuleId}");
        }

        void BackToProfiles()
        {
            Nav.NavigateTo("/posting-profiles");
        }

        void ActivateConfirmed()
        {
            if (SelectedRule == null) return;

            RuleService.Activate(SelectedRule.PostingRuleId);
            ToastService.ShowSuccess($"Rule '{SelectedRule.RuleName}' activated");

            LoadRules();
        }

        void DeactivateConfirmed()
        {
            if (SelectedRule == null) return;

            RuleService.Deactivate(SelectedRule.PostingRuleId);
            ToastService.ShowSuccess($"Rule '{SelectedRule.RuleName}' deactivated");

            LoadRules();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            RuleService.ResetToSeed();
            LoadRules();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Rule list refreshed", "Refreshed");
        }
    }
}
