using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class QuotationSeedData
    {

        public static List<QuotationViewModel> GetSeedQuotations(
            List<CompanyModel> companies,
            List<CustomerViewModel> customers,
            List<ItemViewModel> items,
            string? UserName)
        {
            var quotations = new List<QuotationViewModel>();

            var userId = Guid.NewGuid();
            var userName = UserName ?? "System";

            foreach (var company in companies)
            {
                var companyCustomers = customers
                    .Where(c => c.CompanyId == company.Id)
                    .ToList();

                var companyItems = items
                    .Where(i => i.CompanyId == company.Id && i.Status == ItemStatus.Active)
                    .ToList();

                if (!companyCustomers.Any() || !companyItems.Any())
                    continue;

                for (int i = 0; i < 2; i++)
                {
                    var customer = companyCustomers[i % companyCustomers.Count];

                    var item1 = companyItems.ElementAtOrDefault(0);
                    var item2 = companyItems.ElementAtOrDefault(1);

                    var quotation = new QuotationViewModel
                    {
                        Id = Guid.NewGuid(),

                        CompanyId = company.Id,
                        CompanyName = company.LegalName,

                        Subject = $"Office Equipment Quote {i + 1}",

                        QuotationNumber = GenerateQuotationNumber(quotations.Count + 1),

                        CustomerId = customer.Id,

                        Status = QuotationStatus.New,

                        QuotationDate = DateTime.Today.AddDays(-i),

                        ExpiryDate = DateTime.Today.AddDays(15),

                        OwnerId = userId,
                        OwnerName = userName,

                        TermsAndConditions = "Payment within 15 days.",
                        Description = "Sample seeded quotation",

                        CreatedAt = DateTime.UtcNow,

                        Items = new List<QuotationLineItemViewModel>()
                    };

                    if (item1 != null)
                    {
                        quotation.Items.Add(new QuotationLineItemViewModel
                        {
                            Id = Guid.NewGuid(),
                            ItemId = item1.Id,
                            ItemName = item1.ItemName,
                            UnitId = item1.UnitId,
                            Unit = item1.UnitName,
                            Rate = item1.DefaultRate,
                            TaxPercentage = item1.TaxPercentage,
                            Quantity = 1
                        });
                    }

                    if (item2 != null)
                    {
                        quotation.Items.Add(new QuotationLineItemViewModel
                        {
                            Id = Guid.NewGuid(),
                            ItemId = item2.Id,
                            ItemName = item2.ItemName,
                            UnitId = item2.UnitId,
                            Unit = item2.UnitName,
                            Rate = item2.DefaultRate,
                            TaxPercentage = item2.TaxPercentage,
                            Quantity = 2
                        });
                    }

                    CalculateTotals(quotation);
                    quotations.Add(quotation);
                }
            }

            return quotations;
        }

        private static void CalculateTotals(QuotationViewModel quotation)
        {
            if (quotation.Items == null || !quotation.Items.Any())
                return;

            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var line in quotation.Items)
            {
                var lineAmount = line.Rate * line.Quantity;

                subTotal += lineAmount;
                taxTotal += lineAmount * line.TaxPercentage / 100;
            }

            quotation.SubTotal = subTotal;

            quotation.TaxAmount = taxTotal;

            quotation.DiscountAmount =
                quotation.SubTotal * quotation.Discount / 100;

            quotation.GrandTotal =
                quotation.SubTotal + quotation.TaxAmount - quotation.DiscountAmount;
        }

        private static string GenerateQuotationNumber(int number)
        {
            return $"QT-{DateTime.Now:yyyy}-{number:0000}";
        }
    }
}
