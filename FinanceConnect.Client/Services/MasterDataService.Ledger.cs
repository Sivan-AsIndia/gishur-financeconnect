using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Ledger Methods
        public List<LedgerModel> GetAllLedgers() => _ledgers.Where(l => !l.IsDeleted).ToList();

        public LedgerModel? GetLedgerById(Guid id) => _ledgers.FirstOrDefault(l => l.Id == id && !l.IsDeleted);

        public List<LedgerModel> GetLedgersByCompany(Guid companyId) =>
            _ledgers.Where(l => l.CompanyId == companyId && !l.IsDeleted).ToList();

        public LedgerModel? GetDefaultLedgerByCompany(Guid companyId) =>
            _ledgers.FirstOrDefault(l => l.CompanyId == companyId && l.IsDefaultLedger && !l.IsDeleted);

        public void AddLedger(LedgerModel ledger)
        {
            ledger.Id = Guid.NewGuid();
            ledger.CreatedAt = DateTime.Now;
            ledger.CreatedBy = "System";
            _ledgers.Add(ledger);
        }

        public void UpdateLedger(LedgerModel ledger)
        {
            var existing = _ledgers.FirstOrDefault(l => l.Id == ledger.Id);
            if (existing != null)
            {
                var index = _ledgers.IndexOf(existing);
                ledger.UpdatedAt = DateTime.Now;
                ledger.UpdatedBy = "System";
                _ledgers[index] = ledger;
            }
        }

        public void ActivateLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.Status = "Active";
                ledger.UpdatedAt = DateTime.Now;
                ledger.UpdatedBy = "System";
            }
        }

        public void DeactivateLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.Status = "Inactive";
                ledger.UpdatedAt = DateTime.Now;
                ledger.UpdatedBy = "System";
            }
        }

        public void DeleteLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.IsDeleted = true;
                ledger.DeletedAt = DateTime.Now;
                ledger.DeletedBy = "System";
            }
        }

        public bool CanDeleteLedger(Guid id) => !_generalLedgerEntries.Any(g => g.LedgerId == id && !g.IsDeleted);

        public bool CanDeactivateLedger(Guid id) => !_generalLedgerEntries.Any(g => g.LedgerId == id && !g.IsDeleted);

        public bool CanEditLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            return ledger?.LockStatus == "Unlocked";
        }
    }
}
