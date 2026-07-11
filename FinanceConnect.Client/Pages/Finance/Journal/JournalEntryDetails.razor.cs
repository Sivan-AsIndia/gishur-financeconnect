using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class JournalEntryDetails : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private JournalEntryService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private JournalEntryModel? Entry { get; set; }
        private List<JournalLineModel> Lines { get; set; } = new();

        private bool IsBalanced => Entry != null && Entry.TotalDebit == Entry.TotalCredit;
        private bool CanEdit => Entry?.Status == JournalEntryStatus.Draft;
        private bool CanSubmit => Entry?.Status == JournalEntryStatus.Draft;
        private bool CanApprove => Entry?.Status == JournalEntryStatus.Submitted;
        private bool CanPost => Entry?.Status == JournalEntryStatus.Approved;
        private bool CanCancel => Entry?.Status == JournalEntryStatus.Posted;

        protected override async Task OnInitializedAsync()
        {
            LoadData();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private void LoadData()
        {
            Entry = Service.GetById(Id);
            if (Entry != null)
            {
                Lines = Service.GetLines(Entry.Id);
                Entry.LineCount = Lines.Count;
            }
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private void SubmitEntry()
        {
            try
            {
                Service.Submit(Entry!.Id);
                ToastService.ShowSuccess($"{Entry.JournalEntryNumber} submitted successfully");
                LoadData();
                StateHasChanged();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private void ApproveEntry()
        {
            try
            {
                Service.Approve(Entry!.Id);
                ToastService.ShowSuccess($"{Entry.JournalEntryNumber} approved successfully");
                LoadData();
                StateHasChanged();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private void RejectEntry()
        {
            try
            {
                Service.Reject(Entry!.Id);
                ToastService.ShowSuccess($"{Entry.JournalEntryNumber} rejected");
                LoadData();
                StateHasChanged();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private void PostEntry()
        {
            try
            {
                Service.Post(Entry!.Id);
                ToastService.ShowSuccess($"{Entry.JournalEntryNumber} posted to ledger");
                LoadData();
                StateHasChanged();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private void CancelEntry()
        {
            try
            {
                Service.Cancel(Entry!.Id);
                ToastService.ShowSuccess($"{Entry.JournalEntryNumber} cancelled");
                LoadData();
                StateHasChanged();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private string GetStatusBadgeClass(JournalEntryStatus status) => status switch
        {
            JournalEntryStatus.Draft => "bg-warning-transparent text-warning",
            JournalEntryStatus.Submitted => "bg-info-transparent text-info",
            JournalEntryStatus.Approved => "bg-primary-transparent text-primary",
            JournalEntryStatus.Posted => "bg-success-transparent text-success",
            JournalEntryStatus.Rejected => "bg-danger-transparent text-danger",
            JournalEntryStatus.Cancelled => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
