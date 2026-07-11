using FinanceConnect.Client.Pages;
using static FinanceConnect.Client.Pages.Product_List;

namespace FinanceConnect.Client.Services
{
    public class ProductService
    {
        public List<Product_List.Product> Products { get; set; } = new();

        public void AddProduct(Product_List.Product product)
        {
            Products.Add(product);
        }

        public Product? GetBySku(string sku)
        {
            return Products.FirstOrDefault(p => p.SKU == sku);
        }
    }
}
