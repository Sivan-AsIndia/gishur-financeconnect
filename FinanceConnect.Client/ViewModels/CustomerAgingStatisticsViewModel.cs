namespace FinanceConnect.Client.ViewModels;

public class CustomerAgingStatisticsViewModel
{
    public int TotalSnapshots { get; set; }
    public int CompletedSnapshots { get; set; }
    public int GeneratingSnapshots { get; set; }
    public int FailedSnapshots { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalInvoices { get; set; }
    public int OverdueCustomers { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal BucketCurrentTotal { get; set; }
    public decimal Bucket1To30Total { get; set; }
    public decimal Bucket31To60Total { get; set; }
    public decimal Bucket61To90Total { get; set; }
    public decimal Bucket90PlusTotal { get; set; }
}
