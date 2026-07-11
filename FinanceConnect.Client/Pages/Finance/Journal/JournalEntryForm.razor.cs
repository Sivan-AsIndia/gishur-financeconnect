using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class JournalEntryForm
    {

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] JournalService JournalService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        JournalEntryModel entry = new();
        private EditContext _editContext;

        RichTextEditor? narrationEditor;
        RichTextEditor? overrideReasonEditor;

        bool IsEdit => Id.HasValue;
        bool IsReadOnly => entry.Status == JournalEntryStatus.Posted;

        string LedgerName = "-";

        string FiscalYearName => entry.FiscalYearName ?? "-";
        string PeriodName => entry.AccountingPeriodName ?? "-";

        bool NarrationRequired =>
            Journals.FirstOrDefault(j => j.Id == entry.JournalId)?.NarrationRequired ?? true;

        // ---------- Lookups ----------
        List<BranchModel> Branches = new();
        List<JournalModel> Journals = new();
        JournalModel Journal = new();
        List<CompanyModel> Companies = new();
        CompanyModel? SelectedCompany = new();

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

        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            if (IsEdit)
            {
                entry = Service.GetById(Id!.Value)
                    ?? throw new Exception("Journal Entry not found");

                LoadLookups();

                SetFyAndAccPeriod();
            }
            else
            {
                entry = new JournalEntryModel
                {
                    Id = Guid.NewGuid(),
                    JournalEntryNumber = $"DRAFT-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    EntryDate = DateTime.Today,
                    PostingDate = DateTime.Today,
                    Status = JournalEntryStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    AttachmentCount = 0,
                    CreatedBy = "system"
                };

            }
            _editContext = new EditContext(entry);
        }

        void LoadLookups()
        {
            if (!entry.CompanyId.HasValue) return;

            Branches = Service.GetBranchesByCompany(entry.CompanyId.Value);
            Journals = Service.GetJournalsByCompany(entry.CompanyId.Value);
        }

        void SetFyAndAccPeriod()
        {
            if (!entry.CompanyId.HasValue) return;
            try
            {
                var (fy, period, error) = Service.GetFyByDate(entry);

                if (fy != null)
                {
                    entry.FiscalYearName = fy.FiscalYearName ?? "-";
                    entry.FiscalYearId = fy.Id;
                }
                else
                {
                    entry.FiscalYearName = "-";
                    entry.FiscalYearId = null;
                }

                if (period != null)
                {

                    entry.AccountingPeriodName = period.PeriodName ?? "-";
                    entry.AccountingPeriodId = period.Id;
                    entry.IsPeriodClosed = false;
                }
                else
                {
                    entry.AccountingPeriodName = "-";
                    entry.IsPeriodClosed = true;
                }
                if (error != null)
                {
                    ToastService.ShowError(error);
                    return;
                }


            }
            catch(Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void OnCompanyChanged(ChangeEventArgs e)
        {
            entry.CompanyId = Guid.TryParse(e.Value?.ToString(), out var id)
                ? id
                : null;
            SelectedCompany = Companies.FirstOrDefault(c => c.Id == entry?.CompanyId);
            entry.AccountingPeriodName = "-";
            entry.AccountingPeriodId = null;
            entry.FiscalYearName = "-";
            entry.FiscalYearId = null;
            entry.BranchId = null;
            entry.JournalId = null;
            entry.LedgerId = null;

            Branches.Clear();
            Journals.Clear();

            if (!entry.CompanyId.HasValue) return;

            LoadLookups();
            SetFyAndAccPeriod();
        }

        void OnPostDateCahnge()
        {
            if(entry.PostingDate < DateTime.Today)
            {
                entry.IsBackdated = true;
            }
            SetFyAndAccPeriod();
        }
        void OnEntryDateChange()
        {
            if (entry.EntryDate > entry.PostingDate)
            {
                entry.PostingDate = entry.EntryDate;
                SetFyAndAccPeriod();
            }


        }

        void OnJournalChanged(ChangeEventArgs e)
        {
            entry.JournalId = Guid.TryParse(e.Value?.ToString(), out var id)
                ? id
                : null;

            if (entry.JournalId.HasValue)
            {
                var j = Journals.First(x => x.Id == entry.JournalId);
                entry.LedgerId = j.LedgerId;
                entry.IsReversal = j.AllowReversalEntries;
                LedgerName = JournalService.GetLedgerById(j.LedgerId)?.LedgerName ?? "-";
            }
        }


        // ---------- Actions ----------
        async Task SaveDraft()
        {

            try
            {
                if (narrationEditor != null)
                {
                    entry.Narration = await narrationEditor.GetHtmlAsync();
                }
                if (overrideReasonEditor != null)
                {
                    entry.PostingPolicyOverrideReason = await overrideReasonEditor.GetHtmlAsync();
                }

                entry.Narration = entry.Narration?.Trim() ?? string.Empty;
                if (entry.JournalEntryNumber.StartsWith("DRAFT"))
                {
                     Journal = JournalService.GetAll()
                        .FirstOrDefault(j => j.Id == entry.JournalId)
                        ?? throw new Exception("Journal not found");

                }

                if (NarrationRequired && string.IsNullOrWhiteSpace(entry.Narration))
                {
                    ToastService.ShowError("Narration is required and cannot be blank.");
                    return;
                }
                if (IsEdit)
                {
                    Service.UpdateDraft(entry);
                    ToastService.ShowSuccess("Journal Entry-Draft saved");
                }
                else
                {
                    Service.CreateDraft(entry, Journal);
                    ToastService.ShowSuccess("Journal Entry-Draft Updated");

                }

                Nav.NavigateTo("/journal-entries");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void Submit()
        {
            try
            {
                //Service.CreateDraft(entry);
                Service.Submit(entry.Id);
                ToastService.ShowSuccess("Journal Entry Submitted");
                Reload();
                Nav.NavigateTo("/journal-entries");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void Approve()
        {
            Service.Approve(entry.Id);
            ToastService.ShowSuccess("Journal Entry Approved");
            Reload();
            Nav.NavigateTo("/journal-entries");
        }

        void Post()
        {
            Service.Post(entry.Id);
            ToastService.ShowSuccess("Journal Entry Posted");
            Reload();
            Nav.NavigateTo("/journal-entries");
        }

        void Reload()
        {
            entry = Service.GetById(entry.Id)!;
            LoadLookups();
        }

        void Cancel()
        {
            Nav.NavigateTo("/journal-entries");
        }


    }
}
