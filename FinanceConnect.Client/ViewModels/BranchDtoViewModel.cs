namespace FinanceConnect.Client.ViewModels
{
    public class BranchDto
    {
        public string Id { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;   // CHN
        public string Name { get; set; } = string.Empty;   // Chennai Branch

        public bool IsActive { get; set; } = true;
    }
}
