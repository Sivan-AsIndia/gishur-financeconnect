using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class BankTransactionService
    {
        private readonly List<BankTransactionModel> _transactions = new();
        private bool _seeded;
        private readonly List<BankTransactionModel> _transactionsData = new();
        public BankTransactionService()
        {
            _transactions = BankTransactionSeedData.GetAllTransactions();
        }

        public void EnsureSeeded(Guid bankAccountId)
        {
            if (_seeded) return;

            _transactionsData.AddRange(
                BankTransactionSeedData.GetAllTransactionsMatch(bankAccountId)
            );

            _seeded = true;
        }

        // ===============================
        // GET ALL
        // ===============================
        public Task<List<BankTransactionModel>> GetAllAsync()
        {
            return Task.FromResult(_transactions.ToList());
        }

        // ===============================
        // GET BY ID
        // ===============================
        public Task<BankTransactionModel?> GetByIdAsync(Guid id)
        {
            var item = _transactions.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }

        public List<BankTransactionModel> GetByBankAccount(Guid? bankAccountId)
        {
            return _transactions
                .Where(t =>
                    t.BankAccountId == bankAccountId &&
                    t.PostingStatus != "Reversed")
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.TransactionNumber)
                .ToList();
        }

        public List<BankTransactionModel> GetByBankAccountForMatch(Guid bankAccountId)
        {
            EnsureSeeded(bankAccountId);
            return _transactionsData
                .Where(t =>
                    t.BankAccountId == bankAccountId &&
                    t.PostingStatus != "Reversed")
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.TransactionNumber)
                .ToList();
        }

        // ===============================
        // CREATE
        // ===============================
        public Task CreateAsync(BankTransactionModel model)
        {
            model.Id = Guid.NewGuid();

            _transactions.Add(model);
            return Task.CompletedTask;
        }


        public string GenerateTransactionNumber()
        {
            var next = _transactions.Count + 1;

            return $"BNKTXN-{next.ToString("D6")}";
        }


        // ===============================
        // UPDATE
        // ===============================
        public Task UpdateAsync(Guid id,BankTransactionModel model)
        {
            var existing = _transactions.FirstOrDefault(x => x.Id == id);

            if (existing != null)
            {
                existing.TransactionNumber = model.TransactionNumber;
                existing.TransactionType = model.TransactionType;
                existing.Direction = model.Direction;
                existing.Amount = model.Amount;
                existing.SourceModule = model.SourceModule;
                existing.CurrencyCode = model.CurrencyCode;
                existing.TransactionStatus = model.TransactionStatus;
                existing.TransactionDate = model.TransactionDate;
            }

            return Task.CompletedTask;
        }

        // ===============================
        // DELETE
        // ===============================
        public Task DeleteAsync(Guid id)
        {
            var item = _transactions.FirstOrDefault(x => x.Id == id);

            if (item != null)
                _transactions.Remove(item);

            return Task.CompletedTask;
        }
    }
}
