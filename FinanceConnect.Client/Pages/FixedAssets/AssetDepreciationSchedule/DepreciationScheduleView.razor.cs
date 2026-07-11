using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetDepreciationSchedule
{
    public partial class DepreciationScheduleView
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private AssetDepreciationScheduleService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private AssetDepreciationScheduleViewModel.AssetDepreciationSchedule? Item;
        private string ShowFilter = "";
        private bool isInitialized = false;

        private List<AssetDepreciationScheduleViewModel.AssetDepreciationScheduleLine> FilteredLines =>
            Item?.ScheduleLines == null
                ? new()
                : ShowFilter switch
                {
                    "posted" => Item.ScheduleLines.Where(l => l.IsPosted).ToList(),
                    "unposted" => Item.ScheduleLines.Where(l => !l.IsPosted).ToList(),
                    _ => Item.ScheduleLines
                };

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = await Service.GetByIdAsync(Id);
            isInitialized = true;
        }


        //Slider 
        private const int VisibleCards = 3;
        private int SliderOffset = 0;

        private int SliderMax => Math.Max(0, FilteredLines.Count() - VisibleCards);
        private int SliderTotalPages => (int)Math.Ceiling((double)FilteredLines.Count() / VisibleCards);
        private int SliderPage => (int)Math.Round((double)SliderOffset / VisibleCards) + 1;
        private int PostedPercent => Item?.ScheduleLines.Count > 0
            ? (int)((double)Item.ScheduleLines.Count(l => l.IsPosted) / Item.ScheduleLines.Count * 100) : 0;

        private void SlidePrev() => SliderOffset = Math.Max(0, SliderOffset - VisibleCards);
        private void SlideNext() => SliderOffset = Math.Min(SliderMax, SliderOffset + VisibleCards);
        private string GetStatusBadgeClass(AssetDepreciationScheduleViewModel.ScheduleStatusEnum status) => status switch
        {
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Draft => "bg-warning-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active => "bg-success-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Superseded => "bg-warning-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked => "bg-danger-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Cancelled => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetLockBadgeClass(AssetDepreciationScheduleViewModel.LineLockStatusEnum status) => status switch
        {
            AssetDepreciationScheduleViewModel.LineLockStatusEnum.Open => "bg-success-transparent",
            AssetDepreciationScheduleViewModel.LineLockStatusEnum.LockedPosted => "bg-info-transparent",
            AssetDepreciationScheduleViewModel.LineLockStatusEnum.LockedSuperseded => "bg-warning-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
