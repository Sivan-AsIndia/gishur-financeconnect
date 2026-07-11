using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;


namespace FinanceConnect.Client.Services
{
    public class BankAccountService
    {
        private readonly List<BankAccountModel> _accounts = new();

        private static readonly List<BankAccountModel> _bankAccounts = new();
        [Parameter] public string? id { get; set; }

        public BankAccountService()
        {

            _accounts = BankAccountServiceData.Get();
        }

        public Task<List<BankAccountModel>> GetActiveAccountsAsync()
        {
            var list = _accounts
                .Where(x =>
                    x.BankAccountStatus == "Active" &&
                    !x.IsBlocked &&
                    !x.IsLockedForTransactions)
                .ToList();

            return Task.FromResult(list);
        }


        public Task<BankAccountModel?> GetBankAccountByIdAsync(Guid id)
        {
            var account = _bankAccounts.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(account);
        }


        // Delete a bank account
        public Task DeleteBankAccountAsync(Guid id)
        {
            var existing = _bankAccounts.FirstOrDefault(x => x.Id == id);
            if (existing != null)
                _bankAccounts.Remove(existing);
            return Task.CompletedTask;
        }

        public Task UpdateBankAccountAsync(BankAccountModel model)
        {
            var existing = _accounts.FirstOrDefault(x => x.Id == model.Id);
            if (existing == null) return Task.CompletedTask;

            existing.CompanyId = model.CompanyId;
            existing.BranchId = model.BranchId;
            existing.BranchName = model.BranchName;
            existing.BankAccountCode = model.BankAccountCode;
            existing.BankAccountName = model.BankAccountName;
            existing.Description = model.Description;
            existing.BankName = model.BankName;
            existing.BankBranchName = model.BankBranchName;
            existing.IFSCCode = model.IFSCCode;
            existing.AccountHolderName = model.AccountHolderName;
            existing.BankAccountType = model.BankAccountType;
            existing.IsOverdraftAllowed = model.IsOverdraftAllowed;
            existing.OverdraftLimitAmount = model.OverdraftLimitAmount;
            existing.IsLockedForTransactions = model.IsLockedForTransactions;
            existing.IsBlocked = model.IsBlocked;
            existing.BankGLAccountCode = model.BankGLAccountCode;
            existing.ClearingGLAccountCode = model.ClearingGLAccountCode;
            existing.BankAccountStatus = model.BankAccountStatus;
            existing.CurrencyId = model.CurrencyId;

            return Task.CompletedTask;
        }


        public Task CreateBankAccountAsync(BankAccountModel model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(model.BankAccountNumberEncrypted) &&
                model.BankAccountNumberEncrypted.Length >= 4)
            {
                model.BankAccountNumberLast4 =
                    model.BankAccountNumberEncrypted[^4..];
            }

            _accounts.Add(model);
            return Task.CompletedTask;
        }

     

        // ==============================
        // SAFE SUBSTRING HELPER
        // ==============================
        private string SafeSubstring(string? value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (startIndex < 0 || startIndex >= value.Length) return string.Empty;
            if (startIndex + length > value.Length)
                length = value.Length - startIndex;
            return value.Substring(startIndex, length);
        }

        /* ==============================
           READ
           ============================== */
        public List<BankAccountModel> GetAll() => _accounts;

        public BankAccountModel? GetById(Guid id)
            => _accounts.FirstOrDefault(x => x.Id == id);

        public BankAccountModel? GetByCode(string code)
     => _accounts.FirstOrDefault(x => x.BankAccountCode == code);

        public Task<BankAccountModel?> GetByCodeAsync(string code)
      => Task.FromResult(_accounts.FirstOrDefault(x => x.BankAccountCode == code));


        /* ==============================
           CREATE
           ============================== */
        public void Add(BankAccountModel model)
        {
            // Duplicate prevention (Company + IFSC + Last4)
            if (_accounts.Any(a =>
                a.CompanyId == model.CompanyId &&
                a.IFSCCode == model.IFSCCode &&
                a.BankAccountNumberLast4 == model.BankAccountNumberLast4))
            {
                throw new InvalidOperationException("Duplicate bank account detected");
            }

            model.Id = Guid.NewGuid();
            model.CreatedAt = DateTime.Now;
            model.BankAccountStatus = "Active";

            _accounts.Add(model);
        }

        /* ==============================
           UPDATE
           ============================== */
        public void Update(BankAccountModel model)
        {
            var existing = GetById(model.Id);
            if (existing == null) return;

            if (existing.BankAccountStatus == "Closed")
                throw new InvalidOperationException("Closed bank account cannot be edited");

            existing.BankAccountName = model.BankAccountName;
            existing.Description = model.Description;
            existing.IsOverdraftAllowed = model.IsOverdraftAllowed;
            existing.OverdraftLimitAmount = model.OverdraftLimitAmount;
            existing.IsLockedForTransactions = model.IsLockedForTransactions;
            existing.IsBlocked = model.IsBlocked;
            existing.BankGLAccountCode = model.BankGLAccountCode;
            existing.ClearingGLAccountCode = model.ClearingGLAccountCode;
            existing.BankAccountStatus = model.BankAccountStatus;
        }

        /* ==============================
           LIFECYCLE ACTIONS
           ============================== */

        public void Delete(Guid id, string reason)
        {
            var acc = GetById(id);
            if (acc == null) return;

            // Optional: you can log the reason before removing
            acc.Description += $" Deleted: {reason}";

            // Actually remove from the list
            _accounts.Remove(acc);
        }


        public void Lock(Guid id, string reason)
        {
            var acc = GetById(id);
            if (acc == null) return;

            acc.IsLockedForTransactions = true;
            acc.Description += $" Locked: {reason}";
        }

        public void Unlock(Guid id)
        {
            var acc = GetById(id);
            if (acc == null) return;

            acc.IsLockedForTransactions = false;
            acc.Description += " Unlocked";
        }


      
       
        /* ==============================
           POSTING VALIDATION
           ============================== */
        public void ValidateForPosting(Guid Id, decimal amount)
        {
            var acc = GetById(Id);
            if (acc == null)
                throw new InvalidOperationException("Invalid bank account");

            if (acc.BankAccountStatus != "Active")
                throw new InvalidOperationException("Bank account is not active");

            if (acc.IsLockedForTransactions || acc.IsBlocked)
                throw new InvalidOperationException("Bank account is locked or blocked");

            if (!acc.IsOverdraftAllowed && amount < 0)
                throw new InvalidOperationException("Overdraft not allowed for this account");
        }
    }
}
