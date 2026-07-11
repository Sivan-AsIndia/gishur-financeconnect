namespace FinanceConnect.Client.ViewModels;

public class PaymentTermViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Days { get; set; }
}