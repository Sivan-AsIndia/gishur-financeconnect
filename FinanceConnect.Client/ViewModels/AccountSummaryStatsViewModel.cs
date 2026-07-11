namespace FinanceConnect.Client.ViewModels;

public class AccountSummaryStatsViewModel
{
    public int TotalAccounts { get; set; }
    public int ActiveAccounts { get; set; }
    public int FrozenAccounts { get; set; }
    public int PostingBlockedAccounts { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal TotalUnapplied { get; set; }
    public decimal TotalAdvances { get; set; }
    public int AccountsOverCreditLimit { get; set; }
}
