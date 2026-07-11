using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class TransactionLineSeedData
    {
        // Accept ONE transaction, not a list
        public List<TransactionLineModel> Seed(FinancialTransactionModel tx)
        {
            var lines = new List<TransactionLineModel>();

            decimal principal = Math.Round(tx.TransactionAmount * 0.85m, 2);
            decimal tax = tx.TransactionAmount - principal;

            int lineNo = 10;

            // PRINCIPAL
            lines.Add(new TransactionLineModel
            {
                TransactionLineId = Guid.NewGuid(),
                FinancialTransactionId = tx.FinancialTransactionId,

                TenantId = tx.TenantId,
                BranchId = tx.BranchId,

                LineNumber = lineNo,
                LineType = TransactionLineType.PRINCIPAL,
                PostingCategory = PostingCategory.EXPENSE,
                LineNarration = "Principal Amount",
                Quantity = 1,
                UnitRate = principal,
                LineAmount = principal,
                BaseAmount = principal,

                IsTaxLine = false,
                IsSystemGenerated = true,
                IsAdjustment = false,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            });

            lineNo += 10;

            // TAX
            lines.Add(new TransactionLineModel
            {
                TransactionLineId = Guid.NewGuid(),
                FinancialTransactionId = tx.FinancialTransactionId,

                TenantId = tx.TenantId,
                BranchId = tx.BranchId,

                LineNumber = lineNo,
                LineType = TransactionLineType.TAX,
                PostingCategory = PostingCategory.TAX_INPUT,

                LineNarration = "GST 18%",
                Quantity = 1,
                UnitRate = tax,
                LineAmount = tax,
                BaseAmount = tax,

                IsTaxLine = true,
                IsSystemGenerated = true,
                IsAdjustment = false,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            });

            return lines;
        }
    }
}
