using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionStatus
{
    public partial class TransactionStatusForm
    {
        private EditContext _editContext = default!;
        RichTextEditor? _descriptionEditor;
        [Inject] TransactionStatusService StatusService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        TransactionStatusModel status = new();
        //private List<CompanyLookup> Companies = new();

        public List<CompanyModel> Companies = new();
        // ================= UI STATE =================
        bool ShowIdentity = true;
        bool ShowLifecycle = false;
        bool ShowControls = false;
        bool ShowDisplay = false;
        bool ShowSystem = false;

        bool IsEdit => Id.HasValue;

        bool IdentityTouched = false;
        bool LifecycleTouched = false;
        bool ControlsTouched = false;
        bool DisplayTouched = false;
        bool SystemTouched = false;

        void TouchIdentity() => IdentityTouched = true;
        void TouchLifecycle() => LifecycleTouched = true;
        void TouchControls() => ControlsTouched = true;
        void TouchDisplay() => DisplayTouched = true;
        void TouchSystem() => SystemTouched = true;

        string PageTitle => IsEdit ? "Edit Transaction Status" : "Create Transaction Status";
        string PageSubTitle => IsEdit
            ? "Modify enterprise lifecycle and control rules"
            : "Define finance transaction lifecycle and enforcement policy";

        // ================= LIFECYCLE =================
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JS.InvokeVoidAsync("initFeatherIcons");
        }

        protected override void OnInitialized()
        {
            //Companies = StatusService.GetCompanies();
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            if (IsEdit)
            {
                var existing = StatusService.GetById(Id!.Value);
                if (existing != null)
                {
                    status = new TransactionStatusModel
                    {
                        TransactionStatusId = existing.TransactionStatusId,
                        TenantId = existing.TenantId,
                        CompanyScopeMode = existing.CompanyScopeMode,
                        CompanyId = existing.CompanyId,

                        Code = existing.Code,
                        Name = existing.Name,
                        Description = existing.Description,

                        StageCategory = existing.StageCategory,
                        IsFinal = existing.IsFinal,

                        AllowHeaderEdit = existing.AllowHeaderEdit,
                        AllowLineEdit = existing.AllowLineEdit,
                        AllowDelete = existing.AllowDelete,
                        AllowSubmit = existing.AllowSubmit,
                        AllowApproveReject = existing.AllowApproveReject,
                        AllowPost = existing.AllowPost,
                        AllowReverse = existing.AllowReverse,
                        AllowCancel = existing.AllowCancel,

                        DisplayOrder = existing.DisplayOrder,
                        BadgeLabel = existing.BadgeLabel,
                        BadgeTone = existing.BadgeTone,

                        IsActive = existing.IsActive,
                        IsSystemDefined = existing.IsSystemDefined,

                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy,
                    };
                }
            }
            else
            {
                status = new TransactionStatusModel
                {
                    CompanyScopeMode = CompanyScopeMode.Global,
                    StageCategory = StageCategory.DraftStage,

                    AllowHeaderEdit = true,
                    AllowLineEdit = true,
                    AllowDelete = true,
                    AllowSubmit = true,

                    AllowApproveReject = false,
                    AllowPost = false,
                    AllowReverse = false,
                    AllowCancel = true,

                    DisplayOrder = 0,
                    IsActive = true,
                    IsSystemDefined = false,
                    IsFinal = false
                };
            }

            _editContext = new EditContext(status);
        }

        // ================= INPUT HELPERS =================
        string StatusCodeInput
        {
            get => status.Code;
            set => status.Code = value?.Trim().ToUpperInvariant() ?? "";
        }

        // ================= SUBMIT PIPELINE =================
        private async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                status.Description = await _descriptionEditor.GetHtmlAsync();

            if (_editContext.Validate())
            {
                await ContinueSave();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("identity");
            else if (HasLifecycleErrors())
                OpenAccordion("lifecycle");
            else if (HasControlErrors())
                OpenAccordion("controls");

            await InvokeAsync(StateHasChanged);
        }

        // ================= ACCORDION =================
        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "lifecycle":
                    ShowLifecycle = !ShowLifecycle;
                    break;
                case "controls":
                    ShowControls = !ShowControls;
                    break;
                case "display":
                    ShowDisplay = !ShowDisplay;
                    break;
                case "system":
                    ShowSystem = !ShowSystem;
                    break;
            }
        }

        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = true;
                    break;
                case "lifecycle":
                    ShowLifecycle = true;
                    break;
                case "controls":
                    ShowControls = true;
                    break;
                case "display":
                    ShowDisplay = true;
                    break;
                case "system":
                    ShowSystem = true;
                    break;
            }
        }

        // ================= VALIDATION =================
        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(status.Code)
                || string.IsNullOrWhiteSpace(status.Name)
                || (status.CompanyScopeMode == CompanyScopeMode.PerCompany && status.CompanyId == null);
        }

        bool HasLifecycleErrors()
        {
            return status.StageCategory == 0;
        }

        bool HasControlErrors()
        {
            // Final state cannot allow edits
            if (status.IsFinal &&
                (status.AllowHeaderEdit || status.AllowLineEdit))
                return true;

            // Posting must be locked
            if (status.StageCategory == StageCategory.FinalStage &&
                (status.AllowHeaderEdit || status.AllowLineEdit))
                return true;

            return false;
        }

        // ================= SAVE =================
        async Task ContinueSave()
        {
            try
            {
                if (IsEdit)
                {
                    StatusService.update(status);
                    ToastService.ShowSuccess($"Status '{status.Name}' updated");
                }
                else
                {
                    StatusService.Save(status);
                    ToastService.ShowSuccess($"Status '{status.Name}' created");
                }

                Nav.NavigateTo("/transaction-status");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }

        void OnStatusNameChanged()
        {
            status.Name = status.Name?.Trim() ?? "";
        }

        // ================= NAV =================
        void BackToList()
        {
            ShowIdentity = true;
            ShowLifecycle = false;
            ShowControls = false;
            ShowDisplay = false;
            ShowSystem = false;

            IdentityTouched = false;
            LifecycleTouched = false;
            ControlsTouched = false;
            DisplayTouched = false;
            SystemTouched = false;

            Nav.NavigateTo("/transaction-status");
        }
    }
}
