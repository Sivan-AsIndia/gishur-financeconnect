using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Master.Branch
{
    public partial class AddBranch
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        private EditContext _editContext;

        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        List<CompanyModel> Companies = new();
        CompanyModel? SelectedCompany = new();
        List<CountryModel> Countries = new();
        List<CityModel> Cities = new();
        List<StateProvinceModel> States = new();
        List<TimeZoneModel> TimeZones = new();

        BranchModel branch = new();
        RichTextEditor? _notesEditor;
        RichTextEditor? _hoursNotesEditor;
        bool IdentityTouched = false;
        bool AddressTouched = false;
        bool ContactTouched = false;
        bool FinanceTouched = false;
        bool ReportTouched = false;
        bool StatusTouched = false;

        bool ShowIdentity = true;
        bool ShowAddress = false;
        bool ShowContact = false;
        bool ShowFinance = false;
        bool ShowReport = false;
        bool ShowStatus = false;

        void TouchIdentity() => IdentityTouched = true;
        void TouchAddress() => AddressTouched = true;
        void TouchContact() => ContactTouched = true;
        void TouchFinance() => FinanceTouched = true;
        void TouchReport() => ReportTouched = true;
        void TouchStatus() => StatusTouched = true;
        bool IsEdit => Id.HasValue;
        bool IsInitializing = false;
        bool OriginalIsDefault = false;

        string PageTitle => IsEdit ? "Edit Branch" : "Create Branch";
        string PageSubTitle => IsEdit ? "Update branch details" : "Create new branch";

        private string? CompanyMinDate =>
           SelectedCompany?.BooksStartDate.ToString("yyyy-MM-dd");

        bool ShowOtherBranchType = false;
        List<BranchModel> ParentBranches = new();
        private BranchModel? existing = new();

        string ConfirmTitle = "";
        string ConfirmMessage = "";
        string ConfirmType = "warning";
        Action? ConfirmAction;
        string BranchCodeInput
        {
            get => branch.BranchCode;
            set
            {
                branch.BranchCode = value?.Trim().ToUpperInvariant() ?? "";
            }
        }

        Guid SelectedCountry
        {
            get => branch.CountryId;
            set
            {
                branch.CountryId = value;

                if (!IsInitializing)
                {
                    branch.StateId = Guid.Empty;
                    branch.CityId = Guid.Empty;
                    Cities.Clear();
                    OnChangeCountry();
                }
            }
        }

        Guid SelectedState
        {
            get => branch.StateId;
            set
            {
                branch.StateId = value;
                if (!IsInitializing)
                {

                    OnChangeStates();
                }
            }
        }



        List<string> BranchTypeList = new()
        {
            "Head Office",
            "Regional Office",
            "Factory",
            "Warehouse",
            "Retail Outlet",
            "Project Site"
        };

        string SelectedBranchType
        {
            get => ShowOtherBranchType ? "__OTHER__" : branch.BranchType;
            set
            {
                if (value == "__OTHER__")
                {
                    ShowOtherBranchType = true;
                    branch.BranchType = "";
                }
                else
                {
                    ShowOtherBranchType = false;
                    branch.BranchType = value;
                }
            }
        }


        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active")
            .ToList();
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            TimeZones = MasterDataService.GetAllTimeZones().Where(c => c.IsActive).ToList();
            if (IsEdit)
            {
                 existing = BranchService.GetById(Id!.Value);
                if (existing != null)
                {
                    ParentBranches = BranchService.GetByCompanyId(existing.CompanyId);
                    SelectedCompany = Companies.FirstOrDefault(c => c.Id == existing.CompanyId);
                    IsInitializing = true;

                    branch = new BranchModel
                    {
                        Id = existing.Id,

                        // Identity
                        BranchCode = existing.BranchCode,
                        BranchName = existing.BranchName,
                        Company = existing.Company,
                        CompanyId = existing.CompanyId,
                        BranchType = existing.BranchType,
                        ParentBranchId = existing.ParentBranchId,
                        IsDefaultBranch = existing.IsDefaultBranch,

                        // Address
                        AddressLine1 = existing.AddressLine1,
                        AddressLine2 = existing.AddressLine2,
                        CountryId = existing.CountryId,
                        StateId = existing.StateId,
                        CityId = existing.CityId,
                        PostalCode = existing.PostalCode,
                        TimeZoneId = existing.TimeZoneId,

                        // Contact
                        BranchManagerName = existing.BranchManagerName,
                        BranchEmail = existing.BranchEmail,
                        BranchPhone = existing.BranchPhone,
                        OperatingHoursNote = existing.OperatingHoursNote,

                        // Financial Controls
                        BooksStartDate = existing.BooksStartDate,
                        AllowPostingFrom = existing.AllowPostingFrom,
                        AllowPostingTo = existing.AllowPostingTo,
                        BackdatedPostingDaysAllowed = existing.BackdatedPostingDaysAllowed,
                        FuturePostingDaysAllowed = existing.FuturePostingDaysAllowed,
                        LockBackDatedPosting = existing.LockBackDatedPosting,

                        // Reporting
                        ReportSortOrder = existing.ReportSortOrder,
                        BranchColorTag = existing.BranchColorTag,
                        IsReportingEnabled = existing.IsReportingEnabled,

                        // Status & Notes
                        Status = existing.Status,
                        Notes = existing.Notes,

                        // System
                        CreatedAt = existing.CreatedAt,
                        UpdatedAt = existing.UpdatedAt,
                    };

                    OriginalIsDefault = existing.IsDefaultBranch;

                    InitializeBranchTypeForEdit();
                    LoadCascadeData();
                    // Populate cascades WITHOUT clearing values
                    SelectedCountry = branch.CountryId;
                    SelectedState = branch.StateId;

                    IsInitializing = false;
                }
            }
            _editContext = new EditContext(branch);
        }



        async Task OpenConfirmModal(
        string title,
        string message,
        Action yesAction,
        string type = "warning")
        {
            ConfirmTitle = title;
            ConfirmMessage = message;
            ConfirmType = type;
            ConfirmAction = yesAction;

            await JS.InvokeVoidAsync("bootstrapModal.show", "confirm-modal");
        }

        async Task ConfirmYes()
        {
            var action = ConfirmAction;
            ConfirmAction = null;


            await JS.InvokeVoidAsync("bootstrapModal.hide", "confirm-modal");

            await Task.Delay(150);

            action?.Invoke();
        }
        void OnPostalCodeInput(ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? string.Empty;

            // Allow only letters, digits, space, hyphen
            var sanitized = new string(
                value.Where(c =>
                    char.IsLetterOrDigit(c) ||
                    c == ' ' ||
                    c == '-'
                ).ToArray()
            );

            branch.PostalCode = sanitized;
        }

        void OnChangeCountry()
        {
            branch.StateId = Guid.Empty;
            branch.CityId = Guid.Empty;
            States = MasterDataService.GetStateProvincesByCountry(branch.CountryId).Where(c => c.IsActive).ToList();

        }

        void OnChangeStates()
        {
            branch.CityId = Guid.Empty;
            Cities = MasterDataService.GetCitiesByState(branch.StateId).Where(c => c.IsActive).ToList();

        }


        void OnChangeCompany()
        {
            branch.ParentBranchId = null;
            ParentBranches = BranchService.GetByCompanyId(branch.CompanyId);
            branch.Company  = Companies.FirstOrDefault(c => c.Id == branch.CompanyId)?.LegalName??"-";
            SelectedCompany = Companies.FirstOrDefault(c => c.Id == branch.CompanyId);
        }


        void LoadCascadeData()
        {
            States = MasterDataService.GetStateProvincesByCountry(branch.CountryId).Where(c => c.IsActive).ToList();
            Cities = MasterDataService.GetCitiesByState(branch.StateId).Where(c => c.IsActive).ToList();
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



        private async Task HandleSubmit()
        {
            if (_notesEditor != null)
            {
                branch.Notes = await _notesEditor.GetHtmlAsync();
            }
            if (_hoursNotesEditor != null)
            {
                branch.OperatingHoursNote = await _hoursNotesEditor.GetHtmlAsync();
            }

            if (_editContext.Validate())
            {
                await Save();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("branchIdentity");
            else if (HasAddressErrors())
                OpenAccordion("branchAddress");
            else if (HasContactErrors())
                OpenAccordion("branchContact");
            else if (HasFinanceErrors())
                OpenAccordion("branchFinance");

            await InvokeAsync(StateHasChanged);
        }


        void ToggleAccordion(string section)
        {
            // Determine if the clicked section is currently open
            bool isCurrentlyOpen = section switch
            {
                "branchIdentity" => ShowIdentity,
                "branchAddress" => ShowAddress,
                "branchContact" => ShowContact,
                "branchFinance" => ShowFinance,
                "branchReport" => ShowReport,
                "branchStatus" => ShowStatus,
                _ => false
            };

            // Close all sections first
            ShowIdentity = false;
            ShowAddress = false;
            ShowContact = false;
            ShowFinance = false;
            ShowReport = false;
            ShowStatus = false;

            // If it was closed, open it; if it was open, keep all closed (toggle off)
            if (!isCurrentlyOpen)
            {
                switch (section)
                {
                    case "branchIdentity": ShowIdentity = true; break;
                    case "branchAddress": ShowAddress = true; break;
                    case "branchContact": ShowContact = true; break;
                    case "branchFinance": ShowFinance = true; break;
                    case "branchReport": ShowReport = true; break;
                    case "branchStatus": ShowStatus = true; break;
                }
            }
        }

        void OpenAccordion(string section)
        {
            // Close all first, then open the target
            ShowIdentity = false;
            ShowAddress = false;
            ShowContact = false;
            ShowFinance = false;
            ShowReport = false;
            ShowStatus = false;

            switch (section)
            {
                case "branchIdentity":
                    ShowIdentity = true;
                    break;
                case "branchAddress":
                    ShowAddress = true;
                    break;
                case "branchContact":
                    ShowContact = true;
                    break;
                case "branchFinance":
                    ShowFinance = true;
                    break;
                case "branchReport":
                    ShowReport = true;
                    break;
                case "branchStatus":
                    ShowStatus = true;
                    break;
            }
        }


        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(branch.BranchCode)
                || string.IsNullOrWhiteSpace(branch.BranchName)
                || string.IsNullOrWhiteSpace(branch.Company)
                || string.IsNullOrWhiteSpace(branch.BranchType);
        }

        bool HasAddressErrors()
        {
            return string.IsNullOrWhiteSpace(branch.AddressLine1)
                || branch.CountryId == Guid.Empty
                || branch.StateId == Guid.Empty
                || branch.CityId == Guid.Empty
                || string.IsNullOrWhiteSpace(branch.PostalCode);
        }


        bool HasContactErrors()
        {
            return !string.IsNullOrWhiteSpace(branch.BranchEmail)
                && !branch.BranchEmail.Contains("@");
        }

        bool HasFinanceErrors()
        {
            return branch.BackdatedPostingDaysAllowed < 0
                || branch.FuturePostingDaysAllowed < 0;
        }



        void InitializeBranchTypeForEdit()
        {
            if (string.IsNullOrWhiteSpace(branch.BranchType))
                return;

            if (!BranchTypeList.Any(t =>
                t.Equals(branch.BranchType, StringComparison.OrdinalIgnoreCase)))
            {
                ShowOtherBranchType = true;
            }
            else
            {
                ShowOtherBranchType = false;
            }
        }

        async Task ValidateDefaultBranch()
        {
            if (!IsEdit && branch.IsDefaultBranch && string.IsNullOrEmpty(branch.BranchCode))
            {
                await OpenConfirmModal(
                    "Invalid Default Branch Code",
                    "For a new Default Branch, the Branch Code must be 'HO'.",
                    yesAction: null,
                    type: "warning"
                );
                return;
            }else if (!IsEdit && branch.IsDefaultBranch && branch.BranchCode == "HO")
            {
                await ContinueSave();
            }
            else if(!IsEdit && branch.IsDefaultBranch)
            {
                await OpenConfirmModal(
                    "Invalid Default Branch Code",
                    "For a new Default Branch, the Branch Code must be 'HO'.",
                    yesAction: null,
                    type: "warning"
                );
                return;
            }

        }



        async Task ValidateDuplicateBranchCode()
        {
            bool exists = BranchService.GetAll()
                .Any(b =>
                    b.CompanyId == branch.CompanyId &&
                    b.BranchCode == branch.BranchCode &&
                    b.Id != branch.Id);

            if (exists)
            {
                await OpenConfirmModal(
                    "Duplicate Branch Code",
                    "Branch Code must be unique within the company.",
                    yesAction: null,
                    "danger"
                );
                return;
            }

            if (!IsEdit && branch.IsDefaultBranch)
            {
                await ValidateDefaultBranch();
            }
            else
            {
                await ContinueSave();
            }

        }


        async Task ValidateDefaultChange()
        {
            if (!IsEdit && !branch.IsDefaultBranch)
            {
                await ValidateDuplicateBranchCode();
                return;
            }
            if(branch.IsDefaultBranch && branch.IsDefaultBranch != existing.IsDefaultBranch)
            {
                var existingDefault = BranchService.GetAll()
                    .FirstOrDefault(b =>
                        b.CompanyId == branch.CompanyId &&
                        b.IsDefaultBranch &&
                        b.Id != branch.Id);

                if (existingDefault != null)
                {
                    branch.IsDefaultBranch = OriginalIsDefault;
                    await OpenConfirmModal(
                        "Change Default Branch",
                        $"'{existingDefault.BranchName}' is currently set as the Default Branch. " +
                        "Changing this will update the default branch for this company. Do you want to continue?",
                        async () =>
                        {
                            branch.IsDefaultBranch = true;
                            await ValidateDuplicateBranchCode();
                        }
                    );
                    return;
                }
            }


            await ValidateDuplicateBranchCode();
        }



        async Task Save()
        {

            if (ShowOtherBranchType)
            {
                var newType = branch.BranchType?.Trim();

                if (!string.IsNullOrWhiteSpace(newType))
                {

                    if (!BranchTypeList.Any(t =>
                        t.Equals(newType, StringComparison.OrdinalIgnoreCase)))
                    {
                        BranchTypeList.Add(newType);
                    }

                    branch.BranchType = newType;
                }
            }
            if (branch.AllowPostingFrom.HasValue && branch.AllowPostingTo.HasValue)
            {
                if (branch.AllowPostingFrom >= branch.AllowPostingTo)
                {
                    ToastService.ShowError("AllowPostingFrom should not be earlier then AllowPostingTo");
                    return;
                }
            }
            if (branch.CountryId ==null || branch.CountryId == Guid.Empty)
            {
                ToastService.ShowError("Country is required");
                return;
            }
            else if (branch.StateId == null || branch.StateId == Guid.Empty)
            {
                ToastService.ShowError("State is required");
                return;
            }
            else if (branch.CityId == null || branch.CityId == Guid.Empty)
            {
                ToastService.ShowError("City is required");
                return;
            }
            //if (branch.Company != null)
            //{
            //    branch.CompanyId = Companies.FirstOrDefault(c => c.LegalName == branch.Company)?.Id;
            //}

            await ValidateDefaultChange();
        }

        async Task ContinueSave()
        {
            try
            {
                if (SelectedCompany != null)
                {
                    branch.BooksStartDate ??= SelectedCompany.BooksStartDate;
                    branch.AllowPostingFrom ??= SelectedCompany.AllowPostingFromDate;
                    branch.AllowPostingTo ??= SelectedCompany.AllowPostingToDate;
                    branch.TimeZoneId ??= SelectedCompany.TimeZoneId;
                }

                if (IsEdit)
                {
                    BranchService.Update(branch);
                    ToastService.ShowSuccess($"Branch: {branch.BranchName} updated successfully");
                }
                else
                {
                    branch.Id = Guid.NewGuid();
                    BranchService.Create(branch);
                    ToastService.ShowSuccess($"New Branch {branch.BranchName} added successfully");
                }
                Nav.NavigateTo("/branches");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }




        void BackToList()
        {
            // Clear form state
            branch = new BranchModel();

            IdentityTouched = false;
            AddressTouched = false;
            ContactTouched = false;
            FinanceTouched = false;
            ReportTouched = false;
            StatusTouched = false;

            ShowOtherBranchType = false;
            States.Clear();
            Cities.Clear();
            TimeZones.Clear();

            // Reset validation context
            _editContext = new EditContext(branch);

            // Navigate
            Nav.NavigateTo("/branches");
        }

        private void OnBranchNameInput()
        {
            branch.BranchName = branch.BranchName?.Trim() ?? "";
        }


    }
}
