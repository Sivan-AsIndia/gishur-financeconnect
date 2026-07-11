using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class JournalLineService
    {
        private readonly COADataService _coa;

        public JournalLineService(COADataService coa)
        {
            _coa = coa;
        }

        public void ValidateLine(JournalLineModel line, JournalEntryModel entry)
        {
            if (line.BranchId == Guid.Empty)
                throw new Exception("Branch is mandatory");

            if (line.BranchId != entry.BranchId)
                throw new Exception("Line branch must match header branch");

            if (!line.AccountId.HasValue)
                throw new Exception("Account is required");

            var account = _coa.GetAllAccounts()
                .FirstOrDefault(x => x.Id == line.AccountId.Value)
                ?? throw new Exception("Invalid account");

            if (!account.IsActive)
                throw new Exception("Account is inactive");

            if (!account.IsPostable)
                throw new Exception("Account is not postable");

            if (line.DebitAmount > 0 && line.CreditAmount > 0)
                throw new Exception("Enter either Debit or Credit, not both");

            if (line.DebitAmount == 0 && line.CreditAmount == 0)
                throw new Exception("Either Debit or Credit is required");

            if (line.Amount <= 0)
                throw new Exception("Amount must be greater than zero");
        }
    }
}
