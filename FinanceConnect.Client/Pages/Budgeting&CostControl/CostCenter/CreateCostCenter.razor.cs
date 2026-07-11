using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.CostCenter;

public partial class CreateCostCenter : ComponentBase
{
    [Parameter] public string? Code { get; set; }

    private EditContext _editContext = default!;
    public bool isInitialized = false;
    public bool isSaving = false;
    public bool _submitted = false;

    public CostCenterModel Model = new();

    private List<CurrencyModel> Currencies = new();
    private List<BranchModel> AllBranches = new();
    private List<CostCenterModel> AvailableParents = new();

    // ─── Accordion State ────────────────────────────────────────────
    bool ShowIdentity = true;
    bool ShowClassification = false;
    bool ShowHierarchy = false;
    bool ShowOwnership = false;
    bool ShowFinancial = false;
    bool ShowAllocation = false;
    bool ShowStatus = false;

    // ─── Validation Errors ──────────────────────────────────────────
    string? CodeError = null;
    string? NameError = null;
    string? TypeError = null;
    string? ControlNatureError = null;
    string? UsageModeError = null;
    string? ParentError = null;
    string? OwnerError = null;
    string? CurrencyError = null;
    string? ControlModeError = null;
    string? StatusError = null;

    [Inject] CostCenterService CostCenterService { get; set; } = default!;
    [Inject] BranchService BranchService { get; set; } = default!;
    [Inject] MasterDataService MasterDataService { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;
    [Inject] ToastService ToastService { get; set; } = default!;

    private bool IsEdit => !string.IsNullOrWhiteSpace(Code);

    public string PageTitle => IsEdit ? "Edit Cost Center" : "Create Cost Center";
    public string PageSubTitle => IsEdit ? "Update cost center details" : "Add a new cost center";

    protected override async Task OnParametersSetAsync()
    {
        Currencies = MasterDataService.GetAllCurrencies();
        AllBranches = BranchService.GetAll();

        // Available parents = all active CCs except self (to prevent circular hierarchy)
        AvailableParents = CostCenterService.GetActive();

        if (IsEdit)
        {
            var existing = await CostCenterService.GetByCodeAsync(Code!);
            if (existing == null)
            {
                Nav.NavigateTo("/cost-centers");
                return;
            }

            // Remove self from parent candidates
            AvailableParents = AvailableParents
                .Where(c => c.Id != existing.Id)
                .ToList();

            Model = new CostCenterModel
            {
                Id = existing.Id,
                CompanyId = existing.CompanyId,
                CostCenterCode = existing.CostCenterCode,
                CostCenterName = existing.CostCenterName,
                ShortName = existing.ShortName,
                Description = existing.Description,
                CostCenterType = existing.CostCenterType,
                ControlNature = existing.ControlNature,
                UsageMode = existing.UsageMode,
                IsSharedServiceCenter = existing.IsSharedServiceCenter,
                IsAllocationSourceAllowed = existing.IsAllocationSourceAllowed,
                IsAllocationTargetAllowed = existing.IsAllocationTargetAllowed,
                ParentCostCenterId = existing.ParentCostCenterId,
                ParentCostCenterName = existing.ParentCostCenterName,
                HierarchyLevel = existing.HierarchyLevel,
                HierarchyPath = existing.HierarchyPath,
                DepartmentId = existing.DepartmentId,
                BranchId = existing.BranchId,
                BranchName = existing.BranchName,
                RegionCode = existing.RegionCode,
                BusinessUnitCode = existing.BusinessUnitCode,
                CostCenterOwnerUserId = existing.CostCenterOwnerUserId,
                CostCenterOwnerName = existing.CostCenterOwnerName,
                ResponsibleManagerUserId = existing.ResponsibleManagerUserId,
                ResponsibleManagerName = existing.ResponsibleManagerName,
                FinanceReviewerUserId = existing.FinanceReviewerUserId,
                FinanceReviewerName = existing.FinanceReviewerName,
                ApprovalRoleCode = existing.ApprovalRoleCode,
                EmailDistributionGroup = existing.EmailDistributionGroup,
                DefaultCurrencyId = existing.DefaultCurrencyId,
                DefaultCurrencyCode = existing.DefaultCurrencyCode,
                BudgetControlMode = existing.BudgetControlMode,
                TolerancePercent = existing.TolerancePercent,
                ToleranceAmount = existing.ToleranceAmount,
                AllowNegativeBalance = existing.AllowNegativeBalance,
                IsCapexAllowed = existing.IsCapexAllowed,
                IsOpexAllowed = existing.IsOpexAllowed,
                DefaultBudgetCategoryCode = existing.DefaultBudgetCategoryCode,
                DefaultGLAccountCode = existing.DefaultGLAccountCode,
                ReportingGroupCode = existing.ReportingGroupCode,
                AllocationBaseType = existing.AllocationBaseType,
                DefaultAllocationDriverValue = existing.DefaultAllocationDriverValue,
                CanReceiveSharedCost = existing.CanReceiveSharedCost,
                CanDistributeSharedCost = existing.CanDistributeSharedCost,
                StatisticalKeyReference = existing.StatisticalKeyReference,
                EffectiveFrom = existing.EffectiveFrom,
                EffectiveTo = existing.EffectiveTo,
                CostCenterStatus = existing.CostCenterStatus,
                IsActive = existing.IsActive,
                IsLocked = existing.IsLocked,
                ClosureReason = existing.ClosureReason,
                ReplacedByCostCenterId = existing.ReplacedByCostCenterId,
                ReplacedByCostCenterName = existing.ReplacedByCostCenterName,
                Notes = existing.Notes,
                OperationalRemarks = existing.OperationalRemarks,
            };
        }
        else
        {
            Model = new CostCenterModel
            {
                CostCenterStatus = "",
                CostCenterType = "",
                ControlNature = "",
                UsageMode = "",
                BudgetControlMode = "",
                DefaultCurrencyCode = "INR",
                EffectiveFrom = DateTime.Today,
                IsCapexAllowed = true,
                IsOpexAllowed = true,
                IsAllocationTargetAllowed = true,
                CanReceiveSharedCost = true,
                AllocationBaseType = "None",
            };
        }

        _editContext = new EditContext(Model);
        isInitialized = true;
        await JS.InvokeVoidAsync("feather.replace");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
    }

    void ToggleAccordion(string section)
    {
        switch (section)
        {
            case "identity": ShowIdentity = !ShowIdentity; break;
            case "classification": ShowClassification = !ShowClassification; break;
            case "hierarchy": ShowHierarchy = !ShowHierarchy; break;
            case "ownership": ShowOwnership = !ShowOwnership; break;
            case "financial": ShowFinancial = !ShowFinancial; break;
            case "allocation": ShowAllocation = !ShowAllocation; break;
            case "status": ShowStatus = !ShowStatus; break;
        }
    }

    // ─── Field Change Handlers ──────────────────────────────────────
    void OnCodeChanged()
    {
        Model.CostCenterCode = (Model.CostCenterCode ?? "").Trim().ToUpperInvariant();
        CodeError = null;
    }

    void OnNameChanged() { Model.CostCenterName = (Model.CostCenterName ?? "").Trim(); NameError = null; }
    void OnTypeChanged() { TypeError = null; }
    void OnControlNatureChanged() { ControlNatureError = null; }
    void OnUsageModeChanged() { UsageModeError = null; }
    void OnOwnerChanged() { OwnerError = null; }
    void OnCurrencyChanged() { CurrencyError = null; }
    void OnControlModeChanged() { ControlModeError = null; }
    void OnStatusChanged() { StatusError = null; }

    void OnParentChanged()
    {
        ParentError = null;

        if (Model.ParentCostCenterId.HasValue)
        {
            // Circular hierarchy check
            if (IsEdit && CostCenterService.WouldCreateCircularHierarchy(Model.Id, Model.ParentCostCenterId.Value))
            {
                ParentError = "Circular hierarchy is not allowed.";
                Model.ParentCostCenterId = null;
                return;
            }

            var parent = CostCenterService.GetById(Model.ParentCostCenterId.Value);
            if (parent != null)
            {
                Model.ParentCostCenterName = parent.CostCenterName;
                Model.HierarchyLevel = parent.HierarchyLevel + 1;
                Model.HierarchyPath = $"{parent.HierarchyPath}/{Model.CostCenterCode}";
            }
        }
        else
        {
            Model.ParentCostCenterName = null;
            Model.HierarchyLevel = 1;
            Model.HierarchyPath = Model.CostCenterCode;
        }
    }

    void OnBranchChanged()
    {
        var branch = AllBranches.FirstOrDefault(b => b.Id == Model.BranchId);
        Model.BranchName = branch?.BranchName;
    }

    // ─── Validation ─────────────────────────────────────────────────
    private bool ValidateAllFields()
    {
        bool isValid = true;

        CodeError = null;
        NameError = null;
        TypeError = null;
        ControlNatureError = null;
        UsageModeError = null;
        OwnerError = null;
        CurrencyError = null;
        ControlModeError = null;
        StatusError = null;

        if (string.IsNullOrWhiteSpace(Model.CostCenterCode))
        { CodeError = "Cost Center Code is required"; isValid = false; }
        else if (!IsEdit && CostCenterService.IsCodeDuplicate(Model.CostCenterCode, Model.CompanyId))
        { CodeError = "Cost center code already exists"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.CostCenterName))
        { NameError = "Cost Center Name is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.CostCenterType))
        { TypeError = "Cost Center Type is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.ControlNature))
        { ControlNatureError = "Control Nature is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.UsageMode))
        { UsageModeError = "Usage Mode is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.CostCenterOwnerName))
        { OwnerError = "Cost Center Owner is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.DefaultCurrencyCode))
        { CurrencyError = "Currency is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.BudgetControlMode))
        { ControlModeError = "Budget Control Mode is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.CostCenterStatus))
        { StatusError = "Status is required"; isValid = false; }

        if (Model.EffectiveTo.HasValue && Model.EffectiveTo < Model.EffectiveFrom)
        { StatusError = "Effective To must be >= Effective From"; isValid = false; }

        return isValid;
    }

    // ─── Submit ─────────────────────────────────────────────────────
    public async Task HandleSubmit()
    {
        _submitted = true;

        if (!ValidateAllFields())
        {
            // Open relevant accordion sections with errors
            if (CodeError != null || NameError != null) ShowIdentity = true;
            if (TypeError != null || ControlNatureError != null || UsageModeError != null) ShowClassification = true;
            if (ParentError != null) ShowHierarchy = true;
            if (OwnerError != null) ShowOwnership = true;
            if (CurrencyError != null || ControlModeError != null) ShowFinancial = true;
            if (StatusError != null) ShowStatus = true;

            await InvokeAsync(StateHasChanged);
            return;
        }

        await Save();
    }

    public async Task Save()
    {
        isSaving = true;

        try
        {
            if (IsEdit)
            {
                await CostCenterService.UpdateAsync(Model);
                ToastService.ShowSuccess($"Cost Center '{Model.CostCenterName}' updated successfully", "Updated");
            }
            else
            {
                await CostCenterService.CreateAsync(Model);
                ToastService.ShowSuccess($"Cost Center '{Model.CostCenterName}' created successfully", "Created");
            }

            Nav.NavigateTo("/cost-centers");
        }
        catch (InvalidOperationException ex)
        {
            ToastService.ShowError(ex.Message, "Error");
        }
        finally
        {
            isSaving = false;
        }
    }
}
