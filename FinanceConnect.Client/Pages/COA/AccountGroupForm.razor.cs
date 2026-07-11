using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA
{
    public partial class AccountGroupForm : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        bool isLoading = true;
        bool isSaving = false;
        bool hasAccounts = false;
        string? errorMessage = null;

        AccountGroupViewModel Model = new();
        List<ChartOfAccountsViewModel> ChartOfAccountsList = new();
        List<AccountGroupViewModel> ParentGroups = new();

        RichTextEditor? _descriptionEditor;
        // Accordion visibility state
        bool ShowIdentity = true;
        bool ShowClassification = false;
        bool ShowDefaultBehavior = false;
        bool ShowStatus = false;

        // Touched state
        bool IdentityTouched = false;
        bool ClassificationTouched = false;
        bool DefaultBehaviorTouched = false;
        bool StatusTouched = false;

        // Validation error messages
        string? chartError = null;
        string? codeError = null;
        string? nameError = null;
        string? natureError = null;
        string? balanceBehaviorError = null;
        string? statusError = null;
        string? lockReasonError = null;

        public bool IsEditMode => Id.HasValue;
        public string PageTitle => IsEditMode ? "Edit Account Group" : "Create Account Group";
        public string PageSubTitle => IsEditMode ? "Update group information" : "Create a new account group";

        // Property wrappers
        public string ChartIdWrapper
        {
            get => Model.ChartOfAccountsId == Guid.Empty ? "" : Model.ChartOfAccountsId.ToString();
            set
            {
                Model.ChartOfAccountsId = string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value);
                chartError = null;
            }
        }

        public string ParentIdWrapper
        {
            get => Model.ParentGroupId?.ToString() ?? "";
            set => Model.ParentGroupId = string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
        }

        public string CodeWrapper
        {
            get => Model.GroupCode ?? "";
            set
            {
                Model.GroupCode = value?.Trim().ToUpperInvariant() ?? "";
                IdentityTouched = true;
            }
        }

        public string NameWrapper
        {
            get => Model.GroupName ?? "";
            set
            {
                Model.GroupName = value?.Trim() ?? "";
                IdentityTouched = true;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            ChartOfAccountsList = (await COADataService.GetChartOfAccountsAsync()).ToList();

            if (IsEditMode)
            {
                var existing = await COADataService.GetAccountGroupByIdAsync(Id!.Value);
                if (existing != null)
                {
                    Model = new AccountGroupViewModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        ChartOfAccountsId = existing.ChartOfAccountsId,
                        ChartOfAccountsName = existing.ChartOfAccountsName,
                        ParentGroupId = existing.ParentGroupId,
                        ParentGroupName = existing.ParentGroupName,
                        GroupCode = existing.GroupCode,
                        GroupName = existing.GroupName,
                        Description = existing.Description,
                        AccountNature = existing.AccountNature,
                        StatementType = existing.StatementType,
                        BalanceBehavior = existing.BalanceBehavior,
                        IsControlGroup = existing.IsControlGroup,
                        ReportingCategory = existing.ReportingCategory,
                        HierarchyLevel = existing.HierarchyLevel,
                        DisplayOrder = existing.DisplayOrder,
                        DefaultIsPostable = existing.DefaultIsPostable,
                        DefaultRequiresBranch = existing.DefaultRequiresBranch,
                        DefaultRequiresCostCenter = existing.DefaultRequiresCostCenter,
                        DefaultAllowManualJournal = existing.DefaultAllowManualJournal,
                        Status = existing.Status,
                        LockReason = existing.LockReason,
                        AccountCount = existing.AccountCount,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy
                    };
                    hasAccounts = existing.AccountCount > 0;
                    await LoadParentGroups();
                }
                else
                {
                    Nav.NavigateTo("/account-groups");
                    return;
                }
            }
            else
            {
                Model.AccountNature = "";
                Model.BalanceBehavior = "";
                Model.Status = "";
                Model.DefaultIsPostable = true;
                Model.DefaultAllowManualJournal = true;
                Model.DefaultRequiresBranch = true;
                Model.DefaultRequiresCostCenter = false;
                Model.IsControlGroup = false;
            }

            isLoading = false;
            await Task.Delay(50);
            await JS.InvokeVoidAsync("feather.replace");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        async Task LoadParentGroups()
        {
            if (Model.ChartOfAccountsId == Guid.Empty)
            {
                ParentGroups = new List<AccountGroupViewModel>();
                return;
            }

            ParentGroups = (await COADataService.GetAccountGroupsAsync())
                .Where(g => g.ChartOfAccountsId == Model.ChartOfAccountsId && g.Id != Model.Id)
                .ToList();
        }

        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "classification":
                    ShowClassification = !ShowClassification;
                    break;
                case "defaultBehavior":
                    ShowDefaultBehavior = !ShowDefaultBehavior;
                    break;
                case "status":
                    ShowStatus = !ShowStatus;
                    break;
            }
        }

        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "identity": ShowIdentity = true; break;
                case "classification": ShowClassification = true; break;
                case "defaultBehavior": ShowDefaultBehavior = true; break;
                case "status": ShowStatus = true; break;
            }
        }

        async Task OnChartChanged(ChangeEventArgs e)
        {
            ChartIdWrapper = e.Value?.ToString() ?? "";
            IdentityTouched = true;
            Model.ParentGroupId = null;
            await LoadParentGroups();
        }

        void OnParentChanged(ChangeEventArgs e)
        {
            ParentIdWrapper = e.Value?.ToString() ?? "";
            IdentityTouched = true;
        }

        void OnCodeChanged()
        {
            codeError = null;
        }

        void OnNameChanged()
        {
            nameError = null;
        }

        void OnNatureChanged(ChangeEventArgs e)
        {
            Model.AccountNature = e.Value?.ToString() ?? "";
            ClassificationTouched = true;
            natureError = null;

            // Auto-set BalanceBehavior based on Nature
            if (!string.IsNullOrEmpty(Model.AccountNature))
            {
                Model.BalanceBehavior = Model.AccountNature switch
                {
                    "Asset" or "Expense" => BalanceBehaviors.Debit,
                    "Liability" or "Equity" or "Income" => BalanceBehaviors.Credit,
                    _ => ""
                };
            }
            else
            {
                Model.BalanceBehavior = "";
            }
        }

        void OnBalanceBehaviorChanged(ChangeEventArgs e)
        {
            Model.BalanceBehavior = e.Value?.ToString() ?? "";
            ClassificationTouched = true;
            balanceBehaviorError = null;
        }

        void OnReportingCategoryChanged(ChangeEventArgs e)
        {
            var val = e.Value?.ToString();
            Model.ReportingCategory = string.IsNullOrEmpty(val) ? null : val;
            ClassificationTouched = true;
        }

        void OnStatusChanged2()
        {
            StatusTouched = true;
            statusError = null;
            if (Model.Status != GroupStatuses.Inactive)
            {
                Model.LockReason = null;
                lockReasonError = null;
            }
        }

        void OnLockReasonChanged()
        {
            StatusTouched = true;
            lockReasonError = null;
        }

        string GetStatementType()
        {
            return Model.AccountNature switch
            {
                "Asset" or "Liability" or "Equity" => "Balance Sheet",
                "Income" or "Expense" => "Income Statement",
                _ => "Unknown"
            };
        }

        bool ValidateForm()
        {
            bool isValid = true;
            chartError = null;
            codeError = null;
            nameError = null;
            natureError = null;
            balanceBehaviorError = null;
            statusError = null;
            lockReasonError = null;

            if (Model.ChartOfAccountsId == Guid.Empty)
            {
                chartError = "Chart of Accounts is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.GroupCode))
            {
                codeError = "Group Code is required";
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(Model.GroupCode.Trim(), @"^[A-Za-z0-9_\-]+$"))
            {
                codeError = "Group Code can only contain letters, numbers, underscore (_) and hyphen (-)";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.GroupName))
            {
                nameError = "Group Name is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.AccountNature))
            {
                natureError = "Account Nature is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BalanceBehavior))
            {
                balanceBehaviorError = "Normal Balance Type is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.Status))
            {
                statusError = "Status is required";
                isValid = false;
            }

            // LockReason required when Status = Inactive
            if (Model.Status == GroupStatuses.Inactive && string.IsNullOrWhiteSpace(Model.LockReason))
            {
                lockReasonError = "Inactivation Reason is required when Status is Inactive";
                isValid = false;
            }

            return isValid;
        }

        bool HasIdentityErrors()
        {
            return chartError != null || codeError != null || nameError != null;
        }

        bool HasClassificationErrors()
        {
            return natureError != null || balanceBehaviorError != null;
        }

        bool HasStatusErrors()
        {
            return statusError != null || lockReasonError != null;
        }

        async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                Model.Description = await _descriptionEditor.GetHtmlAsync();
            if (!ValidateForm())
            {
                // Open ALL accordions that have validation errors
                if (HasIdentityErrors())
                    OpenAccordion("identity");
                if (HasClassificationErrors())
                    OpenAccordion("classification");
                if (HasStatusErrors())
                    OpenAccordion("status");

                await InvokeAsync(StateHasChanged);
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                Model.GroupCode = Model.GroupCode?.Trim().ToUpperInvariant();
                Model.GroupName = Model.GroupName?.Trim();
                if (Model.DisplayOrder < 0) Model.DisplayOrder = 0;
                Model.StatementType = GetStatementType();

                var chart = ChartOfAccountsList.FirstOrDefault(c => c.Id == Model.ChartOfAccountsId);
                Model.ChartOfAccountsName = chart?.ChartName;

                if (Model.ParentGroupId.HasValue)
                {
                    var parent = ParentGroups.FirstOrDefault(g => g.Id == Model.ParentGroupId);
                    Model.ParentGroupName = parent?.GroupName;
                    Model.HierarchyLevel = (parent?.HierarchyLevel ?? 0) + 1;
                }
                else
                {
                    Model.HierarchyLevel = 1;
                }

                if (IsEditMode)
                {
                    Model.UpdatedAt = DateTime.Now;
                    Model.UpdatedBy = AuthService.CurrentUser?.UserName ?? "System";
                    await COADataService.UpdateAccountGroupAsync(Model);
                    ToastService.ShowSuccess($"Group '{Model.GroupName}' updated successfully", "Updated");
                }
                else
                {
                    Model.CreatedAt = DateTime.Now;
                    Model.CreatedBy = AuthService.CurrentUser?.UserName ?? "System";
                    await COADataService.AddAccountGroupAsync(Model);
                    ToastService.ShowSuccess($"Group '{Model.GroupName}' created successfully", "Created");
                }

                Nav.NavigateTo("/account-groups");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isSaving = false;
            }
        }

        void NavigateBack()
        {
            Nav.NavigateTo("/account-groups");
        }
    }
}
