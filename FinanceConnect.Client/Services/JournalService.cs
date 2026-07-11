using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.Pages.Finance.Ledger;
using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.Pages.Finance.Journal.JournalForm;

namespace FinanceConnect.Client.Services
{
    public class JournalService
    {
        private readonly MasterDataService _masterDataService;
        private readonly FinanceDataService _financeDataService;
        private readonly DocumentNumberSeriesService _docNumSeriesService;
        private static List<JournalModel> _journals = new();
        private readonly List<JournalModel> _seedJournals = new();
        private  List<CompanyModel> _companies = new();
        private readonly List<LedgerModel> _ledgers = new();
        private readonly List<DocumentNumberSeriesModel> _docNumSeriesList = new();

        public JournalService(MasterDataService masterDataService, FinanceDataService financeDataService, DocumentNumberSeriesService docNumSeriesService)
        {
            _masterDataService = masterDataService;
            _financeDataService = financeDataService;
            _docNumSeriesService = docNumSeriesService;

            _ledgers = _financeDataService.GetAllLedgers();
            _docNumSeriesList = _docNumSeriesService.GetAll();
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            _seedJournals = SeedJournals();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _journals = CloneList(_seedJournals);
        }

        public List<CompanyModel> GetCompanies()
        {
            return _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
        }
        public List<LedgerModel> GetdLedgerByCompany(Guid companyId)
        {
            return _financeDataService.GetAllLedgers().Where(l => l.CompanyId == companyId)
                .ToList();
        }
        
        public LedgerModel? GetLedgerById(Guid? LedgerId)
        {
            return _financeDataService.GetAllLedgers().FirstOrDefault(l => l.Id == LedgerId);
        }

        public List<JournalModel> GetByDocNumSeriesId(Guid DocNumSeriesId)
        {
            return _journals
                .Where(j => j.DocumentNumberSeriesId == DocNumSeriesId)
                .ToList();
        }


        public DocumentNumberSeriesModel? GetDocNumById(Guid? DocumentNumberSeriesId)
        {
            return _docNumSeriesService.GetAll().FirstOrDefault(d => d.DocumentNumberSeriesId == DocumentNumberSeriesId);
        }
        public List<JournalModel> GetByCompany(Guid companyId)
            => _journals
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.JournalCode)
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

        public List<JournalModel> GetAll()
            => _journals
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

        public JournalModel? GetById(Guid id)
            => _journals.FirstOrDefault(x => x.Id == id);



        public void Create(JournalModel j)
        {
            Validate(j, isEdit: false);
            j.CreatedAt = DateTime.UtcNow;
            _journals.Add(j);
        }

        public void Update(JournalModel j)
        {
            var existing = GetById(j.Id)
                ?? throw new Exception("Journal not found");

            if (existing.HasJournalEntries)
            {
                // immutability rules
                j.JournalCode = existing.JournalCode;
                j.LedgerId = existing.LedgerId;
            }

            Validate(j, isEdit: true);
            j.UpdatedAt = DateTime.UtcNow;
            _journals.Remove(existing);
            _journals.Add(j);
        }

        public void Activate(Guid id, string reason)
        {
            var j = GetById(id)!;
            j.Status = JournalStatus.Active;
            j.UpdatedAt = DateTime.UtcNow;
            j.UpdatedBy = "system";
        }

        public void Deactivate(Guid id, string reason)
        {
            var j = GetById(id)!;

            if (j.HasJournalEntries)
                throw new Exception("Journal with entries cannot be deactivated");

            j.Status = JournalStatus.Inactive;
            j.UpdatedAt = DateTime.UtcNow;
            j.UpdatedBy = "system";
        }
        public void Delete(Guid id)
        {
            var journal = _journals.FirstOrDefault(x => x.Id == id);
            if (journal != null)
                _journals.Remove(journal);
        }

        private void Validate(JournalModel j, bool isEdit)
        {
            if (_journals.Any(x =>
                x.CompanyId == j.CompanyId &&
                x.JournalCode == j.JournalCode &&
                (!isEdit || x.Id != j.Id)))
            {
                throw new Exception("Journal code already exists for this company");
            }

            if (j.DefaultBranchMode == BranchDefaultMode.ForceSpecificBranch
                && j.ForcedBranchId == null)
            {
                throw new Exception("Forced Branch is required for this mode");
            }
        }


        private List<JournalModel> SeedJournals()
        {
            var journals = new List<JournalModel>();

            if (!_companies.Any())
                return journals;


            _companies = GetCompanies();

            foreach (var company in _companies)
            {
                var generalLedger = _financeDataService.GetAllLedgers()
                    .FirstOrDefault(l => l.CompanyId == company.Id && l.IsDefaultLedger);

                var salesLedger = _financeDataService.GetAllLedgers()
                    .FirstOrDefault(l => l.CompanyId == company.Id && !l.IsDefaultLedger);

                // ⭐ Get Document Number Series for this company
                var series = _docNumSeriesService.GetAll()
                    .FirstOrDefault(s =>
                        s.CompanyId == company.Id &&
                        s.IsActive &&
                        s.AppliesToEntityType == AppliesToEntityType.JournalEntry);

                if (series == null)
                    throw new InvalidOperationException(
                        $"No active DocumentNumberSeries found for company {company.Id}");

                _journals.AddRange(new List<JournalModel>
        {
            new()
            {
                JournalCode = "GEN",
                JournalName = "General Journal",
                CompanyId = company.Id,
                JournalType = JournalType.General,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                RequireApprovalBeforePosting = true,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "PUR",
                JournalName = "Purchase Journal",
                CompanyId = company.Id,
                JournalType = JournalType.Purchase,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "SAL",
                JournalName = "Sales Journal",
                CompanyId = company.Id,
                JournalType = JournalType.Sales,
                LedgerId = salesLedger != null ? salesLedger.Id : generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "CR",
                JournalName = "Cash Receipt Journal",
                CompanyId = company.Id,
                JournalType = JournalType.CashReceipt,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "CP",
                JournalName = "Cash Payment Journal",
                CompanyId = company.Id,
                JournalType = JournalType.CashPayment,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "BR",
                JournalName = "Bank Receipt Journal",
                CompanyId = company.Id,
                JournalType = JournalType.BankReceipt,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "BP",
                JournalName = "Bank Payment Journal",
                CompanyId = company.Id,
                JournalType = JournalType.BankPayment,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "ADJ",
                JournalName = "Adjustment Journal",
                CompanyId = company.Id,
                JournalType = JournalType.Adjustment,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                AllowBackdatedPostingOverride = true,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            },
            new()
            {
                JournalCode = "OPEN",
                JournalName = "Opening Balance Journal",
                CompanyId = company.Id,
                JournalType = JournalType.OpeningBalance,
                LedgerId = generalLedger.Id,
                Status = JournalStatus.Active,
                EnforceAccountingPeriodOpen = false,
                DocumentNumberSeriesId = series.DocumentNumberSeriesId
            }
        });
            }
            return _journals;
        }


    }

}
