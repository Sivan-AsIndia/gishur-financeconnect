using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class JournalEntryLine
    {

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Parameter] public Guid? EntryId { get; set; }

        JournalEntryModel? Entry;
        List<JournalLineModel> Lines = new();
        List<JournalLineModel> UiDraftLines = new();
        List<AccountViewModel> Accounts = new();
        List<CompanyModel> Companies = new();
        CompanyModel? SelectedCompany = new();

        private bool isInitialized = false;
        bool IsReadOnly => Entry?.Status != JournalEntryStatus.Draft;

        decimal TotalDebit => UiDraftLines.Sum(x => x.DebitAmount);
        decimal TotalCredit => UiDraftLines.Sum(x => x.CreditAmount);
        decimal Difference => TotalDebit - TotalCredit;

        string? CompanyName => Entry == null
            ? ""
            : Companies.FirstOrDefault(c => c.Id == Entry.CompanyId)?.LegalName ?? Entry.CompanyId.ToString();
        string? BranchName =>
            Entry == null
                ? ""
                : EntryService.GetBranchesByCompany(Entry.CompanyId!.Value)
                    .FirstOrDefault(b => b.Id == Entry.BranchId)?.BranchName
                    ?? Entry.BranchId.ToString();

        string? JournalName =>
            Entry == null
                ? ""
                : EntryService.GetJournalsByCompany(Entry.CompanyId!.Value)
                    .FirstOrDefault(j => j.Id == Entry.JournalId)?.JournalName
                    ?? Entry.JournalId.ToString();
        protected override async Task OnParametersSetAsync()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();

            if (!EntryId.HasValue)
            {
                Entry = null;
                Lines = new List<JournalLineModel>();
                UiDraftLines = new List<JournalLineModel>();
                Accounts = new List<AccountViewModel>();
                isInitialized = false;
                return;
            }

            try
            {
                Entry = EntryService.GetById(EntryId.Value);

                if (Entry == null)
                    return;

                Accounts = await EntryService.GetPostableAccounts(Entry.CompanyId!.Value);
                Lines = EntryService.GetByEntry(Entry.Id)
                        ?? new List<JournalLineModel>();
                SelectedCompany = Companies.FirstOrDefault(c => c.Id == Entry.CompanyId);

                UiDraftLines = Lines.Select(x => Clone(x)).ToList();
                isInitialized = true;


            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        JournalLineModel Clone(JournalLineModel x)
        {
            return new JournalLineModel
            {
                Id = x.Id,
                JournalEntryId = x.JournalEntryId,
                LineNumber = x.LineNumber,
                BranchId = x.BranchId,
                AccountId = x.AccountId,
                DebitAmount = x.DebitAmount,
                CreditAmount = x.CreditAmount,
                LineNarration = x.LineNarration,
                BaseCurrencyId = x.BaseCurrencyId
            };
        }


        void OnDebitChanged(JournalLineModel l, ChangeEventArgs e)
        {
            var v = decimal.TryParse(e.Value?.ToString(), out var x) ? x : 0;
            l.DebitAmount = v;
            if (v > 0) l.CreditAmount = 0;
        }

        void OnCreditChanged(JournalLineModel l, ChangeEventArgs e)
        {
            var v = decimal.TryParse(e.Value?.ToString(), out var x) ? x : 0;
            l.CreditAmount = v;
            if (v > 0) l.DebitAmount = 0;
        }

        void SaveLines()
        {
            try
            {
                foreach (var line in UiDraftLines)
                {
                    if (line.Id == Guid.Empty)
                    {
                        line.BranchId = Entry?.BranchId;
                        line.BaseCurrencyId = SelectedCompany?.BaseCurrencyId;
                        EntryService.AddLine(line);
                    }

                    else
                        EntryService.UpdateLine(line);
                }
                EntryService.SaveDraft(Entry.Id);

                // reload from DB
                Lines = EntryService.GetByEntry(Entry!.Id);
                UiDraftLines = Lines.Select(x => Clone(x)).ToList();
                ToastService.ShowSuccess("Journal Line saved Successfully");
                Nav.NavigateTo("/journal-entries");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        //void AddLine()
        //{
        //    if (Entry == null) return;

        //    var newLine = new JournalLineModel
        //    {
        //        JournalEntryId = Entry.Id,
        //        BranchId = Entry.BranchId!.Value,
        //        DebitAmount = 0,
        //        CreditAmount = 0,
        //        LineNarration = ""
        //    };

        //    LineService.AddLine(newLine);
        //    Lines = LineService.GetByEntry(Entry.Id);
        //}

        void AddLine()
        {
            UiDraftLines.Add(new JournalLineModel
            {
                JournalEntryId = Entry!.Id,
                BranchId = Entry.BranchId!.Value,
                LineNumber = UiDraftLines.Count == 0 ? 10 : UiDraftLines.Max(x => x.LineNumber) + 10
            });
        }

        async Task SaveDraft()
        {
            if (Entry == null) return;

            EntryService.SaveDraft(Entry.Id);

            ToastService.ShowSuccess("Journal Entry saved as Draft");

            Nav.NavigateTo("/journal-entries");
        }

        void post()
        {
            ToastService.ShowSuccess("Journal Entry Line Posted Successfully");

            Nav.NavigateTo("/journal-entries");
        }

        void Duplicate(JournalLineModel l)
        {
            UiDraftLines.Add(new JournalLineModel
            {
                JournalEntryId = Entry!.Id,
                BranchId = Entry.BranchId!.Value,
                LineNumber = UiDraftLines.Max(x => x.LineNumber) + 10,
                AccountId = l.AccountId,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                LineNarration = l.LineNarration
            });
        }

        void Delete(JournalLineModel l)
        {
            // if not saved yet
            if (l.Id == Guid.Empty)
            {
                UiDraftLines.Remove(l);
                return;
            }
            try
            {
                EntryService.DeleteLine(l.Id);
                Lines = EntryService.GetByEntry(Entry!.Id);
                UiDraftLines = Lines.Select(x => Clone(x)).ToList();
                ToastService.ShowSuccess("Line Deleted Successfuly");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        private string GetStatusBadge(JournalEntryStatus status)
        {
            return status switch
            {
                JournalEntryStatus.Posted =>
                    "bg-success-transparent text-success",

                JournalEntryStatus.Cancelled =>
                    "bg-danger-transparent text-danger",

                JournalEntryStatus.Draft =>
                    "bg-warning-transparent text-warning",

                JournalEntryStatus.Approved =>
                    "bg-info-transparent text-info",

                JournalEntryStatus.Rejected =>
                    "bg-primary-transparent text-primary",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }

        void Back()
        {
            Nav.NavigateTo("/journal-entries");
        }
    }
}
