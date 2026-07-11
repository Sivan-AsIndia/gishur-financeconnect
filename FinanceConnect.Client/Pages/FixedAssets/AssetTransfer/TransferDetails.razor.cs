using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.AssetTransformViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetTransfer
{
    public partial class TransferDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private AssetTransferService TransferService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ── State ──────────────────────────────────────────────────────────────
        private AssetTransferListDto? Transfer { get; set; }
        private bool isInitialized = false;


        // ── Status Stepper definition ──────────────────────────────────────────
        private record StatusStep(string Label, TransferStatus Status);

        // ── Timeline ──────────────────────────────────────────────────────────
        private record TimelineEntry(string Label, string Note, string DotClass);
        private List<TimelineEntry> Timeline { get; set; } = new();

        // ── Lifecycle ──────────────────────────────────────────────────────────
        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Transfer = await TransferService.GetByIdAsync(Id);
            BuildTimeline();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        // ── Timeline builder ───────────────────────────────────────────────────
        private void BuildTimeline()
        {
            Timeline.Clear();
            if (Transfer == null) return;

            var s = Transfer.TransferStatus;

            Timeline.Add(new("Created", $"Transfer {Transfer.TransferNumber} created", "bg-primary"));

            if (s >= TransferStatus.Submitted)
                Timeline.Add(new("Submitted", "Sent for approval", "bg-info"));

            if (s == TransferStatus.Rejected)
                Timeline.Add(new("Rejected", Transfer.TransferReason ?? "Rejected", "bg-danger"));

            if (s == TransferStatus.Cancelled)
                Timeline.Add(new("Cancelled", "Transfer cancelled", "bg-secondary"));

            if (s >= TransferStatus.Approved &&
                s != TransferStatus.Rejected && s != TransferStatus.Cancelled)
                Timeline.Add(new("Approved", "Approved for transfer", "bg-success"));

            if (s >= TransferStatus.InTransit &&
                s != TransferStatus.Rejected && s != TransferStatus.Cancelled)
                Timeline.Add(new("In Transit", "Asset dispatched", "bg-warning"));

            if (s >= TransferStatus.Received &&
                s != TransferStatus.Rejected && s != TransferStatus.Cancelled)
                Timeline.Add(new("Received", "Asset received at destination", "bg-info"));

            if (s >= TransferStatus.Posted &&
                s != TransferStatus.Rejected && s != TransferStatus.Cancelled)
                Timeline.Add(new("Posted", "Transfer applied to asset record", "bg-success"));

            if (s == TransferStatus.Reversed)
                Timeline.Add(new("Reversed", Transfer.TransferReason ?? "Reversed", "bg-danger"));

            if (s == TransferStatus.Closed)
                Timeline.Add(new("Closed", "Transfer closed", "bg-success"));
        }


        private static bool IsBeforeInOrder(TransferStatus step, TransferStatus current)
        {
            var order = new[]
            {
                TransferStatus.Draft, TransferStatus.Submitted, TransferStatus.Approved,
                TransferStatus.InTransit, TransferStatus.Received,
                TransferStatus.Posted, TransferStatus.Closed,
            };
            return Array.IndexOf(order, step) < Array.IndexOf(order, current);
        }

      

        // ── Badge / Label helpers ──────────────────────────────────────────────
        private static string GetStatusLabel(TransferStatus s) => s switch
        {
            TransferStatus.Draft => "Draft",
            TransferStatus.Submitted => "Submitted",
            TransferStatus.Approved => "Approved",
            TransferStatus.Rejected => "Rejected",
            TransferStatus.Cancelled => "Cancelled",
            TransferStatus.InTransit => "In Transit",
            TransferStatus.Received => "Received",
            TransferStatus.Posted => "Posted",
            TransferStatus.Reversed => "Reversed",
            TransferStatus.Closed => "Closed",
            _ => "Unknown"
        };

        private static string GetStatusBadgeClass(TransferStatus s) => s switch
        {
            TransferStatus.Draft => "bg-warning-transparent",
            TransferStatus.Submitted => "bg-info-transparent",
            TransferStatus.Approved => "bg-primary-transparent",
            TransferStatus.Rejected => "bg-danger-transparent",
            TransferStatus.Cancelled => "bg-secondary-transparent text-secondary",
            TransferStatus.InTransit => "bg-warning-transparent",
            TransferStatus.Received => "bg-info-transparent",
            TransferStatus.Posted => "bg-success-transparent",
            TransferStatus.Reversed => "bg-secondary-transparent text-secondary",
            TransferStatus.Closed => "bg-success-transparent",
            _ => "bg-light"
        };

        private static string GetTypeLabel(TransferType? t) => t switch
        {
            TransferType.CustodianChange => "Custodian Change",
            TransferType.LocationChange => "Location Change",
            TransferType.BranchChange => "Branch Change",
            TransferType.FullReassignment => "Full Reassignment",
            _ => "—"
        };

    }
}
