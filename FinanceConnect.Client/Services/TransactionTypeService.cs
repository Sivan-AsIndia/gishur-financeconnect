using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Services
{
    public class TransactionTypeService
    {
        private static List<TransactionTypeModel> _types = new();
        private readonly List<TransactionTypeModel> _seedTypes = new();
        private readonly List<CompanyModel> _companies = new();
        private readonly PostingProfileService _postingProfileService;
        private readonly MasterDataService _masterDataService;
        private readonly DocumentNumberSeriesService _docNumberSeriesService;

        public TransactionTypeService(
            MasterDataService masterDataService,
            PostingProfileService postingProfileService, DocumentNumberSeriesService DocumentNumberSeriesService)
        {
            _masterDataService = masterDataService;
            _postingProfileService = postingProfileService;
            _docNumberSeriesService = DocumentNumberSeriesService;

            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            _seedTypes = SeedTypes();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _types = CloneList(_seedTypes);
        }

        public List<CompanyModel> GetCompanies()
        {
            return _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
        } 

        public List<TransactionTypeModel> GetAll() =>
            _types
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

        public List<TransactionTypeModel> GetByProfile(Guid profileId)
        {
            return _types
                .Where(r => r.DefaultPostingProfileId == profileId)
                .ToList();
        }

        public List<TransactionTypeModel> GetByDocNumSeriesId(Guid DocNumSeriesId)
        {
            return _types
                .Where(r => r.DocumentNumberSeriesId == DocNumSeriesId)
                .ToList();
        }

        public TransactionTypeModel? GetById(Guid id) =>
            _types.FirstOrDefault(x => x.TransactionTypeId == id);

        // ================= CREATE =================
        public void Create(TransactionTypeModel model)
        {
            Validate(model, isNew: true);

            model.TransactionTypeId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;

            _types.Add(model);
        }

        // ================= UPDATE =================
        public void Update(TransactionTypeModel model)
        {
            var existing = GetById(model.TransactionTypeId);
            if (existing == null)
                throw new Exception("Transaction type not found");

            Validate(model, isNew: false);

            model.UpdatedAt = DateTime.UtcNow;

            _types.Remove(existing);
            _types.Add(model);
        }

        // ================= ACTIVATE =================
        public void Activate(Guid id)
        {
            var type = GetById(id);
            if (type == null) return;

            type.IsActive = true;
            type.UpdatedAt = DateTime.UtcNow;
        }

        // ================= DEACTIVATE =================
        public bool CanDeactivate(Guid id)
        {
            var type = GetById(id);
            return type != null;
        }

        public void Deactivate(Guid id)
        {
            var type = GetById(id);
            if (type == null) return;

            if (!CanDeactivate(id))
                throw new Exception("Cannot deactivate: active transactions exist");

            type.IsActive = false;
            type.UpdatedAt = DateTime.UtcNow;
        }

        // ================= DELETE =================
        public void Delete(Guid id)
        {
            var type = GetById(id);
            if (type == null) return;

            //if (type.UsageCount > 0)
            //    throw new Exception("Cannot delete: referenced by transactions");

            _types.Remove(type);
        }

        // ================= VALIDATION =================
        private void Validate(TransactionTypeModel model, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
                throw new Exception("Code is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new Exception("Name is required");

            if (model.CompanyId == Guid.Empty)
                throw new Exception("Company is required");

            if (model.IsPostable && model.DefaultPostingProfileId == null)
                throw new Exception("Posting Profile is required when IsPostable = true");

            if (model.DocumentNumberSeriesId == Guid.Empty)
                throw new Exception("Document Series is required");

            if (model.RequiresApproval &&
                string.IsNullOrWhiteSpace(model.ApprovalWorkflowKey))
                throw new Exception("Approval workflow required");

            if (isNew)
            {
                if (_types.Any(x =>
                    x.CompanyId == model.CompanyId &&
                    x.Code.Equals(model.Code, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Transaction Type code must be unique per company");
                }
            }
        }


        // ================= SEED =================
        private List<TransactionTypeModel> SeedTypes()
        {
            var profiles = _postingProfileService.GetAll();
            var DocNumSeries = _docNumberSeriesService.GetAll();

            var seeded = TransactionTypeSeedData.SeedForCompanies(
                _companies,
                profiles,
                DocNumSeries
            );

            return seeded;
        }
    }
}
