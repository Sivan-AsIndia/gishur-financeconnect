namespace FinanceConnect.Client.ViewModels;

/// <summary>
/// AR Adjustment statistics model
/// </summary>
public class ARAdjustmentStatisticsViewModel
{
    public int TotalAdjustments { get; set; }
    public int DraftAdjustments { get; set; }
    public int SubmittedAdjustments { get; set; }
    public int ApprovedAdjustments { get; set; }
    public int PostedAdjustments { get; set; }
    public int CancelledAdjustments { get; set; }
    public int ReversedAdjustments { get; set; }
    public int PendingApproval { get; set; }
    public decimal TotalAdjustmentAmount { get; set; }
    public decimal TotalWriteOffAmount { get; set; }
    public decimal TotalRoundingAmount { get; set; }
}
