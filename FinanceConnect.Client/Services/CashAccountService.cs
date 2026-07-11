using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Data;

namespace FinanceConnect.Client.Services
{
    public class CashAccountService
    {

        private readonly List<CashAccountModels> _accounts = new();
        public CashAccountService()
        {

            _accounts = CashAccountServiceData.Get();

        }

        public List<CashAccountModels> GetAll()
            => _accounts;

        public void Add(CashAccountModels model)
        {
            _accounts.Add(model);
        }

        public CashAccountModels? GetByCode(string code)
        {
            return _accounts.FirstOrDefault(x => x.Code == code);
        }

        public void Delete(string code)
        {
            var account = _accounts.FirstOrDefault(x => x.Code == code);
            if (account != null)
                _accounts.Remove(account);
        }



        public void Update(CashAccountModels model)
        {
            var existing = GetByCode(model.Code);
            if (existing == null) return;

            existing.Name = model.Name;
            existing.BranchId = model.BranchId;
            existing.BranchName = model.BranchName;
            existing.CustodianName = model.CustodianName;
            existing.CurrencyCode = model.CurrencyCode;
            existing.CashGlAccount = model.CashGlAccount;
            existing.MaxCashLimit = model.MaxCashLimit;
            existing.Status = model.Status;
            existing.Description = model.Description;
        }
    }
}
