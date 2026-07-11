using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class TransactionStatusService
    {
        private static List<TransactionStatusModel> _status = new();
        private static List<TransactionStatusModel> _seedStatus = new();
        private readonly TransactionStatusSeed _statusSeed = new();

        private readonly Guid TenantId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        public TransactionStatusService()
        {
            _statusSeed.Seed(TenantId, _status);
            _seedStatus = _status;
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _status = CloneList(_seedStatus);
        }

        public List<TransactionStatusModel> GetAll(Guid tenantId, Guid? companyId = null)
        {
            return _status.Where(s=>s.IsDeleted == false)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }

        public TransactionStatusModel? GetById(Guid id)
        {
            return _status.FirstOrDefault(s => s.TransactionStatusId == id);
        }

        public TransactionStatusModel? GetByCode(Guid tenantId, string code)
        {
            return _status.FirstOrDefault(s =>
                s.TenantId == tenantId &&
                s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        // ================= COMMAND =================
        public void Save(TransactionStatusModel model)
        {
            Validate(model);
            model.CreatedAt = DateTime.UtcNow;
            _status.Add(model);

        }

        public void update(TransactionStatusModel model)
        {
            Validate(model);

            var existing = GetById(model.TransactionStatusId);
            model.UpdatedAt = DateTime.UtcNow;
            if (existing == null)
            {
                _status.Add(model);
            }
            else
            {
                Copy(model, existing);
            }
        }


        public void Delete(Guid transactionStatusId)
        {
            var existing = GetById(transactionStatusId);

            if (existing == null)
                throw new Exception("Transaction status not found");

            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = "Admin";
        }
        public void Activate(Guid id)
        {
            var s = GetById(id);
            if (s != null)
                s.IsActive = true;
        }

        public void Deactivate(Guid id)
        {
            var s = GetById(id);
            if (s != null && !s.IsSystemDefined)
                s.IsActive = false;
        }

        // ================= VALIDATION =================
        private void Validate(TransactionStatusModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
                throw new Exception("Status Code is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new Exception("Status Name is required");

            if (model.DisplayOrder < 0)
                throw new Exception("Display Order must be >= 0");

            bool duplicate = _status.Any(x =>
                x.TransactionStatusId != model.TransactionStatusId &&
                x.TenantId == model.TenantId &&
                x.Code.Equals(model.Code, StringComparison.OrdinalIgnoreCase) &&
                (x.CompanyId == model.CompanyId));

            if (duplicate)
                throw new Exception("Status Code must be unique per scope");

            if (model.IsFinal)
            {
                if (model.AllowHeaderEdit || model.AllowLineEdit)
                    throw new Exception("Final status cannot allow header or line edits");
            }

            if (model.StageCategory == StageCategory.PostingStage && model.AllowHeaderEdit)
                throw new Exception("Posting stage must not allow editing");
        }



        private void Copy(TransactionStatusModel src, TransactionStatusModel dest)
        {
            dest.Code = src.Code;
            dest.Name = src.Name;
            dest.Description = src.Description;
            dest.StageCategory = src.StageCategory;
            dest.IsFinal = src.IsFinal;

            dest.AllowHeaderEdit = src.AllowHeaderEdit;
            dest.AllowLineEdit = src.AllowLineEdit;
            dest.AllowDelete = src.AllowDelete;
            dest.AllowSubmit = src.AllowSubmit;
            dest.AllowApproveReject = src.AllowApproveReject;
            dest.AllowPost = src.AllowPost;
            dest.AllowReverse = src.AllowReverse;
            dest.AllowCancel = src.AllowCancel;
            dest.IsSystemDefined = src.IsSystemDefined;

            dest.DisplayOrder = src.DisplayOrder;
            dest.BadgeLabel = src.BadgeLabel;
            dest.BadgeTone = src.BadgeTone;

            dest.IsActive = src.IsActive;
            dest.UpdatedAt = DateTime.UtcNow;
            dest.UpdatedBy = "Admin";
        }
    }
}
