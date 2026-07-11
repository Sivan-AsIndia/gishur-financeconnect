
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Cost_Allocation
{
    public partial class CostAllocationDetails
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private CostAllocationService Service { get; set; } = default!;

        private CostAllocationListDto? Item;

        protected override void OnInitialized()
            => Item = Service.GetById(Id);

        private string GetStatusBadge(AllocationStatus s) => s switch
        {
            AllocationStatus.Draft => "bg-secondary-transparent text-secondary",
            AllocationStatus.Prepared => "bg-info-transparent",
            AllocationStatus.Submitted => "bg-warning-transparent",
            AllocationStatus.Approved => "bg-success-transparent",
            AllocationStatus.Applied => "bg-success-transparent",
            AllocationStatus.Locked => "bg-danger-transparent",
            AllocationStatus.Closed => "bg-dark",
            AllocationStatus.Reversed => "bg-secondary-transparent text-secondary",
            AllocationStatus.Archived => "bg-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetLineStatusBadge(AllocationLineStatus s) => s switch
        {
            AllocationLineStatus.Draft => "bg-secondary-transparent text-secondary",
            AllocationLineStatus.Calculated => "bg-info-transparent",
            AllocationLineStatus.Approved => "bg-success-transparent",
            AllocationLineStatus.Applied => "bg-success-transparent",
            AllocationLineStatus.Locked => "bg-danger-transparent",
            AllocationLineStatus.Reversed => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
