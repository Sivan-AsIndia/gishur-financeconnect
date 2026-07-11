using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class DeliveryChallanSeedData
    {
        public static List<DeliveryChallanViewModel> GetSeedDeliveryChallans(
            List<CompanyModel> companies,
            List<CustomerViewModel> customers,
            List<ItemViewModel> items)
        {
            var challans = new List<DeliveryChallanViewModel>();


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

                    var challan = new DeliveryChallanViewModel
                    {
                        Id = Guid.NewGuid(),

                        CompanyId = company.Id,
                        CompanyName = company.LegalName,

                        ChallanNumber = GenerateChallanNumber(challans.Count + 1),

                        CustomerId = customer.Id,

                        ChallanDate = DateTime.Today.AddDays(-i),
                        ShippingDate = DateTime.Today.AddDays(-i),

                        PONumber = $"PO-{DateTime.Now:yyyy}-{i + 1:000}",

                        TermsAndConditions = "Goods once delivered cannot be returned.",
                        PrivateNotes = "Seed delivery challan for testing",

                        CreatedAt = DateTime.UtcNow,

                        Items = new List<DeliveryChallanLineItemViewModel>()
                    };

                    if (item1 != null)
                    {
                        challan.Items.Add(new DeliveryChallanLineItemViewModel
                        {
                            Id = Guid.NewGuid(),
                            ItemId = item1.Id,
                            ItemName = item1.ItemName,
                            UnitId = item1.UnitId,
                            Unit = item1.UnitName,
                            Rate = item1.DefaultRate,
                            TaxPercentage = item1.TaxPercentage,
                            Quantity = 1,
                        });
                    }

                    if (item2 != null)
                    {
                        challan.Items.Add(new DeliveryChallanLineItemViewModel
                        {
                            Id = Guid.NewGuid(),
                            ItemId = item2.Id,
                            ItemName = item2.ItemName,
                            UnitId = item2.UnitId,
                            Unit = item2.UnitName,
                            Rate = item2.DefaultRate,
                            TaxPercentage = item2.TaxPercentage,
                            Quantity = 2,
                        });
                    }

                    CalculateTotals(challan);

                    challans.Add(challan);
                }
            }

            return challans;
        }

        private static void CalculateTotals(DeliveryChallanViewModel challan)
        {
            if (challan.Items == null || !challan.Items.Any())
                return;

            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var line in challan.Items)
            {
                var lineAmount = line.Rate * line.Quantity;

                subTotal += lineAmount;

                taxTotal += lineAmount * line.TaxPercentage / 100;
            }

            challan.SubTotal = subTotal;

            challan.TaxAmount = taxTotal;

            challan.GrandTotal = subTotal + taxTotal;
        }

        private static string GenerateChallanNumber(int number)
        {
            return $"DC-{DateTime.Now:yyyy}-{number:0000}";
        }
    }
}