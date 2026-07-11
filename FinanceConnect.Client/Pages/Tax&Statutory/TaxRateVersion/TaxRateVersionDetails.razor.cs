using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxRateVersionViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxRateVersion
{
    public partial class TaxRateVersionDetails
    {

        [Parameter] public Guid Id { get; set; }

        [Inject] private TaxRateVersionService RateVersionService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private TaxRateVersionListDto? Version { get; set; }
        private bool isInitialized = false;

        private record TimelineEntry(string Label, string Note, string DotClass);
        private List<TimelineEntry> Timeline { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Version = await RateVersionService.GetByIdAsync(Id);
            BuildTimeline();
            isInitialized = true;
        }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void BuildTimeline()
        {
            Timeline.Clear();
            if (Version == null) return;

            Timeline.Add(new("Created",
                $"V{Version.VersionNumber} created on {Version.CreatedAt:dd MMM yyyy}", "bg-primary"));

            if (Version.Status >= VersionStatus.Submitted)
                Timeline.Add(new("Submitted", "Sent for approval", "bg-info"));

            if (Version.Status == VersionStatus.Approved || Version.Status > VersionStatus.Approved)
                Timeline.Add(new("Approved",
                    Version.ApprovedOn.HasValue ? $"Approved on {Version.ApprovedOn.Value:dd MMM yyyy}" : "Approved",
                    "bg-success"));

            if (Version.Status == VersionStatus.Active)
                Timeline.Add(new("Activated",
                    Version.ActivatedOn.HasValue ? $"Active from {Version.ActivatedOn.Value:dd MMM yyyy}" : "Active",
                    "bg-success"));

            if (Version.Status == VersionStatus.Retired)
                Timeline.Add(new("Retired",
                    Version.EffectiveTo.HasValue ? $"Retired, closed {Version.EffectiveTo.Value:dd MMM yyyy}" : "Retired",
                    "bg-secondary"));

            if (Version.Status == VersionStatus.Superseded)
                Timeline.Add(new("Superseded", "Replaced by a newer version", "bg-danger"));

            if (Version.Status == VersionStatus.Cancelled)
                Timeline.Add(new("Cancelled", "Version cancelled", "bg-secondary"));

            if (Version.IsLockedForChanges)
                Timeline.Add(new("Locked", Version.LockReason ?? "Locked for immutability", "bg-warning"));

            if (Version.UpdatedAt.HasValue)
                Timeline.Add(new("Updated", $"Last updated {Version.UpdatedAt.Value:dd MMM yyyy}", "bg-info"));
        }

        private static string GetStatusLabel(VersionStatus s) => TaxRateVersion.GetStatusLabel(s);
        private static string GetStatusPillClass(VersionStatus s) => TaxRateVersion.GetStatusPillClass(s);
        private static string GetSourceLabel(RateSourceType? s) => TaxRateVersion.GetSourceLabel(s);
        private static string GetSourceBadgeClass(RateSourceType? s) => TaxRateVersion.GetSourceBadgeClass(s);
    }
}
