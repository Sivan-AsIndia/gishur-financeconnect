using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxCodeViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCode
{
    public partial class TaxCodeDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private TaxCodeService TaxCodeService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ── State ──────────────────────────────────────────────────────────────
        private TaxCodeViewModel.TaxCodeListDto? TaxCode { get; set; }
        private bool isInitialized = false;

        // ── Timeline ──────────────────────────────────────────────────────────
        private record TimelineEntry(string Label, string Note, string DotClass);
        private List<TimelineEntry> Timeline { get; set; } = new();

        // ── Lifecycle ──────────────────────────────────────────────────────────
        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            TaxCode = await TaxCodeService.GetByIdAsync(Id);
            BuildTimeline();
            isInitialized = true;
        }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        // ── Timeline Builder ───────────────────────────────────────────────────
        private void BuildTimeline()
        {
            Timeline.Clear();
            if (TaxCode == null) return;

            Timeline.Add(new("Created", $"Tax code {TaxCode.Code} created on {TaxCode.CreatedAt:dd MMM yyyy}", "bg-primary"));

            if (TaxCode.Status == TaxCodeStatus.Inactive)
                Timeline.Add(new("Inactivated", "Tax code marked as inactive", "bg-warning"));

            if (TaxCode.Status == TaxCodeStatus.Archived)
                Timeline.Add(new("Archived", "Tax code archived", "bg-secondary"));

            if (TaxCode.IsLockedForChanges)
                Timeline.Add(new("Locked", TaxCode.LockReason ?? "Locked for audit integrity", "bg-warning"));

            if (TaxCode.UpdatedAt.HasValue)
                Timeline.Add(new("Updated", $"Last updated on {TaxCode.UpdatedAt.Value:dd MMM yyyy}", "bg-info"));
        }

        // ── Badge / Label Helpers ──────────────────────────────────────────────
        private static string GetTypeLabel(TaxType? t) => t switch
        {
            TaxType.GST => "GST",
            TaxType.TDS => "TDS",
            TaxType.TCS => "TCS",
            TaxType.Other => "Other",
            _ => "—"
        };

        private static string GetTypeBadgeClass(TaxType? t) => t switch
        {
            TaxType.GST => "bg-primary-transparent",
            TaxType.TDS => "bg-warning-transparent",
            TaxType.TCS => "bg-info-transparent",
            TaxType.Other => "bg-secondary-transparent",
            _ => "bg-light"
        };

        private static string GetDirectionLabel(TaxDirection? d) => d switch
        {
            TaxDirection.Input => "Input",
            TaxDirection.Output => "Output",
            TaxDirection.WithholdingPayable => "Withholding Payable",
            TaxDirection.Other => "Other",
            _ => "—"
        };

        private static string GetDirectionBadgeClass(TaxDirection? d) => d switch
        {
            TaxDirection.Input => "bg-success-transparent",
            TaxDirection.Output => "bg-danger-transparent",
            TaxDirection.WithholdingPayable => "bg-purple-transparent text-purple",
            _ => "bg-light"
        };

        private static string GetStatusLabel(TaxCodeStatus s) => s switch
        {
            TaxCodeStatus.Active => "Active",
            TaxCodeStatus.Inactive => "Inactive",
            TaxCodeStatus.Archived => "Archived",
            _ => "Unknown"
        };

        private static string GetStatusBadgeClass(TaxCodeStatus s) => s switch
        {
            TaxCodeStatus.Active => "bg-success-transparent",
            TaxCodeStatus.Inactive => "bg-warning-transparent",
            TaxCodeStatus.Archived => "bg-secondary-transparent text-secondary",
            _ => "bg-light"
        };
    }
}
