using FinanceConnect.Client.Pages.Master.Company;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class JournalForm
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] DocumentNumberSeriesService DocNumberSeriesService { get; set; } = default!;

        List<CompanyModel> CompanyList = new();
        List<BranchModel> Branches = new();
        List<LedgerModel> LedgerList = new();
        List<DocumentNumberSeriesModel> DocNumSeriesList = new();

        [Parameter] public Guid? Id { get; set; }

        JournalModel journal = new();

        DateTime today = DateTime.UtcNow.Date;

        RichTextEditor? _descriptionEditor;
        RichTextEditor? _narrationTemplateEditor;

        bool IdentityTouched = false;
        bool LedgerTouched = false;
        bool NumberingTouched = false;
        bool PostingTouched = false;
        bool NarrationTouched = false;

        void TouchIdentity() => IdentityTouched = true;
        void TouchLedger() => LedgerTouched = true;
        void TouchNumbering() => NumberingTouched = true;
        void TouchPosting() => PostingTouched = true;
        void TouchNarration() => NarrationTouched = true;
        bool IsEdit => Id.HasValue;

        bool IsCodeLocked => IsEdit && journal.HasJournalEntries;
        bool IsLedgerLocked => IsEdit && journal.HasJournalEntries;

        bool IsPrefixManuallyEdited = false;

        private EditContext _editContext;
        bool ShowIdentity = true;
        bool ShowLedger, ShowNumbering, ShowPosting, ShowNarration;



        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                var existing = Service.GetById(Id!.Value);
                if (existing != null)
                {

                    journal = new JournalModel
                    {
                        Id = existing.Id,

                        // Identity
                        JournalCode = existing.JournalCode,
                        JournalName = existing.JournalName,
                        CompanyId = existing.CompanyId,
                        Description = existing.Description,
                        Status = existing.Status,

                        // Type & Ledger
                        JournalType = existing.JournalType,
                        LedgerId = existing.LedgerId,
                        DefaultBranchMode = existing.DefaultBranchMode,
                        ForcedBranchId = existing.ForcedBranchId,

                        // Numbering
                        EntryNumberPrefix = existing.EntryNumberPrefix,
                        ResetNumbering = existing.ResetNumbering,
                        AllowManualEntryNumber = existing.AllowManualEntryNumber,

                        // Posting Controls
                        RequireApprovalBeforePosting = existing.RequireApprovalBeforePosting,
                        EnforceAccountingPeriodOpen = existing.EnforceAccountingPeriodOpen,
                        AllowBackdatedPostingOverride = existing.AllowBackdatedPostingOverride,
                        AllowFuturePostingOverride = existing.AllowFuturePostingOverride,
                        AllowReversalEntries = existing.AllowReversalEntries,
                        MaxLinesPerEntry = existing.MaxLinesPerEntry,

                        // Narration & Attachments
                        NarrationRequired = existing.NarrationRequired,
                        AttachmentRequired = existing.AttachmentRequired,
                        AllowLineLevelNarration = existing.AllowLineLevelNarration,
                        NarrationTemplate = existing.NarrationTemplate,

                        // System
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy,
                    };

                    LoadLookups();
                }
                else
                {
                    ToastService.ShowError("Journal not found");
                }
            }
            else
            {
                journal = new JournalModel
                {
                    Id = Guid.NewGuid(),

                    // Defaults
                    Status = JournalStatus.Draft,
                    RequireApprovalBeforePosting = true,
                    NarrationRequired = true,
                    AllowReversalEntries = true,
                    MaxLinesPerEntry = 500,
                    ResetNumbering = NumberResetFrequency.Yearly,
                    AllowManualEntryNumber = false,

                    // System
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                CompanyList = Service.GetCompanies();

            }
            _editContext = new EditContext(journal);
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

        void LoadLookups()
        {
            if (!journal.CompanyId.HasValue)
            {
                LedgerList.Clear();
                Branches.Clear();
                return;
            }

            var companyId = journal.CompanyId.Value;

            CompanyList = Service.GetCompanies();

            LedgerList = Service.GetdLedgerByCompany(companyId);

            DocNumSeriesList = DocNumberSeriesService.GetAll()
                .Where(l =>
                    l.CompanyId == companyId &&
                    l.AppliesToEntityType == AppliesToEntityType.JournalEntry &&
                    l.IsActive &&
                    (l.EffectiveFrom == null || l.EffectiveFrom.Value.Date <= today) &&
                    (l.EffectiveTo == null || l.EffectiveTo.Value.Date >= today))
                .ToList();
            Branches = BranchService.GetByCompany(companyId);

            StateHasChanged();
        }

        private async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                journal.Description = await _descriptionEditor.GetHtmlAsync();

            if (_narrationTemplateEditor != null)
                journal.NarrationTemplate = await _narrationTemplateEditor.GetHtmlAsync();

            if (_editContext.Validate())
            {
                Save();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("journalIdentity");
            else if (HasLedgerErrors())
                OpenAccordion("journalLedger");
            else if (HasNumberingErrors())
                OpenAccordion("journalNumbering");
            else if (HasPostingErrors())
                OpenAccordion("journalPosting");
            else if (HasNarrationErrors())
                OpenAccordion("journalNarration");

            await InvokeAsync(StateHasChanged);
        }

        bool HasIdentityErrors()
        {
            return _editContext.GetValidationMessages(() => journal.JournalCode).Any()
                || _editContext.GetValidationMessages(() => journal.JournalName).Any()
                || _editContext.GetValidationMessages(() => journal.CompanyId).Any();
        }

        bool HasLedgerErrors()
        {
            return _editContext.GetValidationMessages(() => journal.LedgerId).Any()
                || _editContext.GetValidationMessages(() => journal.DefaultBranchMode).Any()
                || _editContext.GetValidationMessages(() => journal.ForcedBranchId).Any()
                || _editContext.GetValidationMessages(() => journal.JournalType).Any();
        }

        bool HasNumberingErrors()
        {
            return _editContext.GetValidationMessages(() => journal.EntryNumberPrefix).Any()
                || _editContext.GetValidationMessages(() => journal.ResetNumbering).Any();
        }
        bool HasPostingErrors()
        {
            return _editContext.GetValidationMessages(() => journal.MaxLinesPerEntry).Any();
        }
        bool HasNarrationErrors()
        {
            return _editContext.GetValidationMessages(() => journal.NarrationTemplate).Any();
        }


        void ToggleAccordion(string section)
        {
            // Determine if the clicked section is currently open
            bool isCurrentlyOpen = section switch
            {
                "journalIdentity" => ShowIdentity,
                "journalLedger" => ShowLedger,
                "journalNumbering" => ShowNumbering,
                "journalPosting" => ShowPosting,
                "journalNarration" => ShowNarration,
                _ => false
            };

            // Close all sections first
            ShowIdentity = false;
            ShowLedger = false;
            ShowNumbering = false;
            ShowPosting = false;
            ShowNarration = false;

            // If it was closed, open it; if it was open, keep all closed (toggle off)
            if (!isCurrentlyOpen)
            {
                switch (section)
                {
                    case "journalIdentity": ShowIdentity = true; break;
                    case "journalLedger": ShowLedger = true; break;
                    case "journalNumbering": ShowNumbering = true; break;
                    case "journalPosting": ShowPosting = true; break;
                    case "journalNarration": ShowNarration = true; break;
                }
            }
        }

        void OpenAccordion(string section)
        {
            // Close all first, then open the target
            ShowIdentity = false;
            ShowLedger = false;
            ShowNumbering = false;
            ShowPosting = false;
            ShowNarration = false;

            switch (section)
            {
                case "journalIdentity":
                    ShowIdentity = true;
                    break;
                case "journalLedger":
                    ShowLedger = true;
                    break;
                case "journalNumbering":
                    ShowNumbering = true;
                    break;
                case "journalPosting":
                    ShowPosting = true;
                    break;
                case "journalNarration":
                    ShowNarration = true;
                    break;
            }
        }


        void OnJournalCodeChanged(ChangeEventArgs e)
        {
            journal.JournalCode = e.Value?.ToString()?.Trim().ToUpper() ?? "";

            // Auto-fill prefix ONLY if user has not manually edited it
            if (!IsPrefixManuallyEdited && !IsEdit)
            {
                journal.EntryNumberPrefix = journal.JournalCode;
            }
        }

        void OnNameChanged()
        {
            journal.JournalName = journal.JournalName?.Trim() ?? "";
        }

        void OnCompanyChanged(ChangeEventArgs e)
        {
            journal.CompanyId = Guid.TryParse(e.Value?.ToString(), out var id)
                ? id
                : null;

            journal.LedgerId = null;
            journal.ForcedBranchId = null;

            Branches.Clear();

            if (!journal.CompanyId.HasValue)
                return;

            var companyId = journal.CompanyId.Value;
            LedgerList = Service.GetdLedgerByCompany(companyId);

            DocNumSeriesList = DocNumberSeriesService.GetAll()
                .Where(l =>
                    l.CompanyId == companyId &&
                    l.AppliesToEntityType == AppliesToEntityType.JournalEntry &&
                    l.IsActive &&
                    (l.EffectiveFrom == null || l.EffectiveFrom.Value.Date <= today) &&
                    (l.EffectiveTo == null || l.EffectiveTo.Value.Date >= today))
                .ToList();
            Branches = BranchService.GetByCompany(companyId);

            StateHasChanged();
        }



        void Save()
        {
            try
            {
                ValidateJournalUI();

                if (IsEdit)
                {
                    Service.Update(journal);
                    ToastService.ShowSuccess($"Journal '{journal.JournalName}' updated successfully", "Success");
                }
                else
                {
                    Service.Create(journal);
                    ToastService.ShowSuccess($"Journal '{journal.JournalName}' added successfully", "Success");
                }


                Nav.NavigateTo("/journals");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message, "Error");
            }

        }

        void ValidateJournalUI()
        {
            if (journal.DefaultBranchMode == BranchDefaultMode.ForceSpecificBranch
                && journal.ForcedBranchId == null)
                throw new Exception("Forced Branch is required");

            if (journal.MaxLinesPerEntry <= 0)
                throw new Exception("Max lines must be greater than zero");
        }

        void Cancel()
        {
            Nav.NavigateTo("/journals");
        }

    }
}
