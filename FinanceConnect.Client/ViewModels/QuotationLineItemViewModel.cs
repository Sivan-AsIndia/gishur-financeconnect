using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class QuotationLineItemViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid? ItemId { get; set; }

        public string? ItemName { get; set; }

        public Guid? UnitId { get; set; }
        public string? Unit { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Rate { get; set; }

        [Range(1, double.MaxValue)]
        public decimal Quantity { get; set; }

        // Base amount
        public decimal Amount => Rate * Quantity;

        // Tax %
        public decimal TaxPercentage { get; set; }

        // Tax value
        public decimal TaxAmount => Math.Round((Amount * TaxPercentage) / 100, 2);

        // Final line total
        public decimal TotalAmount => Amount + TaxAmount;
    }
}