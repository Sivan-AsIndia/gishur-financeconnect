namespace FinanceConnect.Client.ViewModels;

public class BucketSummaryViewModel
{
    public decimal CurrentAmount { get; set; }
    public decimal Days1To30Amount { get; set; }
    public decimal Days31To60Amount { get; set; }
    public decimal Days61To90Amount { get; set; }
    public decimal Days90PlusAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int CurrentCount { get; set; }
    public int Days1To30Count { get; set; }
    public int Days31To60Count { get; set; }
    public int Days61To90Count { get; set; }
    public int Days90PlusCount { get; set; }
    public int TotalCount => CurrentCount + Days1To30Count + Days31To60Count + Days61To90Count + Days90PlusCount;
}
