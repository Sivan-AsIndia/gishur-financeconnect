using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Opening Balance Methods
        public List<OpeningBalanceModel> GetAllOpeningBalances() => _openingBalances.Where(o => !o.IsDeleted).ToList();

        public OpeningBalanceModel? GetOpeningBalanceById(Guid id) => _openingBalances.FirstOrDefault(o => o.Id == id && !o.IsDeleted);

        public List<OpeningBalanceModel> GetOpeningBalancesByCompany(Guid companyId) =>
            _openingBalances.Where(o => o.CompanyId == companyId && !o.IsDeleted).ToList();

        public List<OpeningBalanceModel> GetOpeningBalancesByBranch(Guid branchId) =>
            _openingBalances.Where(o => o.BranchId == branchId && !o.IsDeleted).ToList();

        public void AddOpeningBalance(OpeningBalanceModel ob)
        {
            ob.Id = Guid.NewGuid();
            ob.OpeningBalanceNumber = $"OB-{DateTime.Now:yyyy}-{_openingBalances.Count + 1:D5}";
            ob.CreatedAt = DateTime.Now;
            ob.CreatedBy = "System";
            _openingBalances.Add(ob);
        }

        public void UpdateOpeningBalance(OpeningBalanceModel ob)
        {
            var existing = _openingBalances.FirstOrDefault(o => o.Id == ob.Id);
            if (existing != null)
            {
                var index = _openingBalances.IndexOf(existing);
                ob.UpdatedAt = DateTime.Now;
                ob.UpdatedBy = "System";
                _openingBalances[index] = ob;
            }
        }

        public void SubmitOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == "Draft" && ob.IsBalanced)
            {
                ob.Status = "Submitted";
                ob.UpdatedAt = DateTime.Now;
                ob.UpdatedBy = "System";
            }
        }

        public void ApproveOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == "Submitted")
            {
                ob.Status = "Approved";
                ob.ApprovedBy = "Controller";
                ob.ApprovedAt = DateTime.Now;
                ob.UpdatedAt = DateTime.Now;
                ob.UpdatedBy = "System";
            }
        }

        public void PostOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == "Approved")
            {
                ob.Status = "Posted";
                ob.PostedBy = "System";
                ob.PostedAt = DateTime.Now;
                ob.UpdatedAt = DateTime.Now;
                ob.UpdatedBy = "System";
            }
        }

        public void CancelOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status != "Posted")
            {
                ob.Status = "Cancelled";
                ob.UpdatedAt = DateTime.Now;
                ob.UpdatedBy = "System";
            }
        }

        public bool CanEditOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            return ob?.Status == "Draft";
        }

        public bool CanDeleteOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            return ob?.Status == "Draft" || ob?.Status == "Cancelled";
        }
    }
}
