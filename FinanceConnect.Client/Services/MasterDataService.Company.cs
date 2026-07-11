using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Company Methods
        public List<CompanyModel> GetAllCompanies() => _companies.Where(c => !c.IsDeleted).ToList();

        public CompanyModel? GetCompanyById(Guid id) => _companies.FirstOrDefault(c => c.Id == id && !c.IsDeleted);

        public CompanyModel? GetCompanyByCode(string code) => _companies.FirstOrDefault(c => c.CompanyCode == code && !c.IsDeleted);

        public void AddCompany(CompanyModel company)
        {
            company.Id = Guid.NewGuid();
            company.CreatedAt = DateTime.Now;
            company.IsDeleted = false;
            _companies.Add(company);
        }

        public void UpdateCompany(CompanyModel company)
        {
            var existing = _companies.FirstOrDefault(c => c.Id == company.Id);
            if (existing != null)
            {
                var index = _companies.IndexOf(existing);
                company.UpdatedAt = DateTime.Now;
                _companies[index] = company;
            }
        }

        public void ActivateCompany(Guid id)
        {
            var company = _companies.FirstOrDefault(c => c.Id == id);
            if (company != null)
            {
                company.IsActive = true;
                company.Status = "Active";
                company.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateCompany(Guid id)
        {
            var company = _companies.FirstOrDefault(c => c.Id == id);
            if (company != null)
            {
                company.IsActive = false;
                company.Status = "Inactive";
                company.UpdatedAt = DateTime.Now;
            }
        }

        public void DeleteCompany(Guid id)
        {
            var company = _companies.FirstOrDefault(c => c.Id == id);
            if (company != null)
            {
                company.IsActive = false;
                company.IsDeleted = true;
                company.DeletedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateCompany(Guid id)
        {
            // In a real app, check if company has active branches or transactions
            return true;
        }

        public bool CanDeleteCompany(Guid id)
        {
            // In a real app, check if company has any references
            return true;
        }

    }
}
