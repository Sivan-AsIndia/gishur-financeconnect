using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.TaxAuditTrailViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxAuditTrailService
    {
        private readonly List<TaxAuditTrailModel> _trails;

        public TaxAuditTrailService()
        {
            _trails = TaxAuditTrailSeedData.Get();
        }

        // ── List / Query (Read-only) ──
        public List<TaxAuditTrailModel> GetList()
            => _trails.OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByEntityType(string entityType)
            => _trails.Where(x => x.EntityType == entityType)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByEntityId(Guid entityId)
            => _trails.Where(x => x.EntityId == entityId)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByEntity(string entityType, Guid entityId)
            => _trails.Where(x => x.EntityType == entityType && x.EntityId == entityId)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByEventCategory(string category)
            => _trails.Where(x => x.EventCategory == category)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByEventType(string eventType)
            => _trails.Where(x => x.EventType == eventType)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetBySeverity(string severity)
            => _trails.Where(x => x.EventSeverity == severity)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetSecurityEvents()
            => _trails.Where(x => x.SecurityEventFlag)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByPeriod(string taxPeriodKey)
            => _trails.Where(x => x.TaxPeriodKey == taxPeriodKey)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByCorrelationId(string correlationId)
            => _trails.Where(x => x.CorrelationId == correlationId)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public List<TaxAuditTrailModel> GetByDateRange(DateTime from, DateTime to)
            => _trails.Where(x => x.EventDate >= from.Date && x.EventDate <= to.Date)
                      .OrderByDescending(x => x.EventTimestamp).ToList();

        public TaxAuditTrailModel? GetById(Guid id)
            => _trails.FirstOrDefault(x => x.Id == id);

        // ── No Create/Update/Delete exposed to UI - system-written only ──
    }
}
