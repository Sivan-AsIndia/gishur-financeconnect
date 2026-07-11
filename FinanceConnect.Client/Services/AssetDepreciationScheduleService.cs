using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AssetDepreciationScheduleService
    {
        private readonly List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> _schedules;

        public AssetDepreciationScheduleService()
        {
            _schedules = AssetDepreciationScheduleSeedData.GetAll();
        }

        /* ================= READ ================= */

        public List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> GetAll()
            => _schedules;

        public AssetDepreciationScheduleViewModel.AssetDepreciationSchedule? GetById(Guid id)
            => _schedules.FirstOrDefault(x => x.AssetDepreciationScheduleId == id);

        public Task<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> GetByAssetId(Guid assetId)
            => _schedules.Where(x => x.FixedAssetId == assetId && !x.IsDeleted).ToList();

        /* ================= GENERATE SCHEDULE ================= */

        public Task GenerateScheduleAsync(AssetDepreciationScheduleViewModel.AssetDepreciationSchedule model)
        {
            if (model.FixedAssetId == null || model.FixedAssetId == Guid.Empty)
                throw new InvalidOperationException("Asset is not in service; schedule cannot be generated.");

            if (model.UsefulLifeMonthsSnapshot <= 0)
                throw new InvalidOperationException("Useful Life must be > 0.");

            model.AssetDepreciationScheduleId = Guid.NewGuid();
            model.ScheduleNumber = $"FASCH-{(_schedules.Count + 1):D6}";
            model.ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active;
            model.ScheduleVersion = 1;
            model.GeneratedOn = DateTime.UtcNow;
            model.GeneratedBy = "System";
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _schedules.Add(model);
            return Task.CompletedTask;
        }

        /* ================= REGENERATE ================= */

        public Task RegenerateAsync(Guid scheduleId, string reason)
        {
            var existing = GetById(scheduleId);
            if (existing == null) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Regeneration requires a reason.");

            if (existing.ScheduleStatus == AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked)
                throw new InvalidOperationException("Schedule is locked/superseded.");

            existing.ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Superseded;
            existing.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        /* ================= LOCK / UNLOCK ================= */

        public Task LockAsync(Guid id, string lockReason)
        {
            var sch = GetById(id);
            if (sch == null) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(lockReason))
                throw new InvalidOperationException("Lock reason is required.");

            sch.ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked;
            sch.LockedOn = DateTime.UtcNow;
            sch.LockReason = lockReason;
            sch.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var sch = GetById(id);
            if (sch == null) return Task.CompletedTask;

            sch.ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active;
            sch.LockedOn = null;
            sch.LockReason = null;
            sch.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var sch = GetById(id);
            if (sch == null) return Task.CompletedTask;

            if (sch.ScheduleStatus == AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked)
                throw new InvalidOperationException("Locked schedule cannot be deleted.");

            if (sch.ScheduleLines.Any(l => l.IsPosted))
                throw new InvalidOperationException("Schedule with posted lines cannot be deleted.");

            sch.IsDeleted = true;
            sch.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
