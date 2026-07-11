using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TDSDeductionEntryViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TDSDeductionEntry
{
    public partial class TDSDeductionEntryDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private TDSDeductionEntryService DeductionService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private TDSDeductionEntryListDto? Entry { get; set; }
        private TDSDeductionEntry? EntryData { get; set; }

        private bool isInitialized = false;

        private record TimelineEntry(string Label, string Note, string DotClass);
        private List<TimelineEntry> Timeline { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Entry = await DeductionService.GetByIdAsync(Id);
            BuildTimeline();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }
        private void BuildTimeline()
        {
            Timeline.Clear();
            if (Entry == null) return;

            Timeline.Add(new("Created",
                $"Entry {Entry.DeductionNumber} created on {Entry.CreatedAt:dd MMM yyyy}",
                "bg-primary"));

            if (Entry.PostedOn.HasValue)
                Timeline.Add(new("Posted",
                    $"Posted on {Entry.PostedOn.Value:dd MMM yyyy}",
                    "bg-info"));

            if (Entry.ThresholdTriggeredFlag)
                Timeline.Add(new("Threshold Crossed",
                    $"Cumulative threshold triggered for {Entry.SectionCodeSnapshot}",
                    "bg-warning"));

            if (Entry.IsAlternatePanRateApplied)
                Timeline.Add(new("Alt PAN Rate Applied",
                    "PAN not available — higher alternate rate used",
                    "bg-danger"));

            if (Entry.SettlementStatus == SettlementStatus.PartiallySettled)
                Timeline.Add(new("Partially Settled",
                    $"₹{Entry.SettledAmount:N0} of ₹{Entry.DeductionAmount:N0} remitted",
                    "bg-warning"));

            if (Entry.SettlementStatus == SettlementStatus.FullySettled)
                Timeline.Add(new("Fully Settled",
                    Entry.LastSettlementDate.HasValue
                        ? $"Fully remitted on {Entry.LastSettlementDate.Value:dd MMM yyyy}"
                        : "All TDS remitted to government",
                    "bg-success"));

            if (Entry.Status == DeductionStatus.Reversed)
                Timeline.Add(new("Reversed",
                    Entry.IsSystemReversal ? "System-generated reversal on source reversal" : "Manually reversed",
                    "bg-danger"));

            if (Entry.Status == DeductionStatus.Cancelled)
                Timeline.Add(new("Cancelled", "Draft entry cancelled", "bg-secondary"));

            if (Entry.UpdatedAt.HasValue)
                Timeline.Add(new("Updated",
                    $"Last updated {Entry.UpdatedAt.Value:dd MMM yyyy}",
                    "bg-info"));
        }
    }
}
