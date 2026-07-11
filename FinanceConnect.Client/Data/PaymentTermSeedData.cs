using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class PaymentTermSeedData
    {
        public static List<PaymentTermViewModel> GetSeedData()
        {
            return new List<PaymentTermViewModel>
            {
                new PaymentTermViewModel { Id = MasterDataIds.PaymentTerms.Net30,     Name = "Net 30 Days", Days = 30 },
                new PaymentTermViewModel { Id = MasterDataIds.PaymentTerms.Net45,     Name = "Net 45 Days", Days = 45 },
                new PaymentTermViewModel { Id = MasterDataIds.PaymentTerms.Net60,     Name = "Net 60 Days", Days = 60 },
                new PaymentTermViewModel { Id = MasterDataIds.PaymentTerms.Immediate, Name = "Immediate",   Days = 0 },
                new PaymentTermViewModel { Id = MasterDataIds.PaymentTerms.Net15,     Name = "Net 15 Days", Days = 15 },
            };
        }
    }
}
