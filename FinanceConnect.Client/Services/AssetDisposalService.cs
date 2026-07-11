using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AssetDisposalService
    {

        private readonly AssetDisposalSeedData _seed;

        private static List<AssetDisposalViewModel> _store = new();

        public AssetDisposalService()
        {
            _seed = new AssetDisposalSeedData();


            if (!_store.Any())
                _store = _seed.Seed();
        }

        public List<AssetDisposalViewModel> GetAll()
        {
            return _store.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public Task<AssetDisposalViewModel?> GetByIdAsync(Guid id)
        {
            var item = _store.FirstOrDefault(x => x.AssetDisposalId == id);
            return Task.FromResult(item);
        }

        public Task CreateAsync(AssetDisposalViewModel model)
        {
            model.AssetDisposalId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(model.DisposalNumber))
                model.DisposalNumber = GenerateNumber();

            _store.Add(model);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(AssetDisposalViewModel model)
        {
            var existing = _store.FirstOrDefault(x => x.AssetDisposalId == model.AssetDisposalId);

            if (existing == null)
                throw new Exception("Asset disposal not found.");

            if (existing.DisposalStatus != AssetDisposalStatus.Draft)
                throw new Exception("Only Draft disposal can be edited.");

            existing.DisposalType = model.DisposalType;
            existing.DisposalDate = model.DisposalDate;
            existing.ProceedsAmount = model.ProceedsAmount;
            existing.DisposalExpenseAmount = model.DisposalExpenseAmount;
            existing.ReferenceNumber = model.ReferenceNumber;
            existing.BuyerName = model.BuyerName;
            existing.Narration = model.Narration;

            existing.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }


        public static string GenerateNumber()
        {
            int next = _store.Count + 1;
            return $"DSP-{next:000000}";
        }

        public void Submit(Guid id)
        {
            var d = _store.First(x => x.AssetDisposalId == id);
            d.DisposalStatus = AssetDisposalStatus.Submitted;
        }

        public void Approve(Guid id)
        {
            var d = _store.First(x => x.AssetDisposalId == id);
            d.DisposalStatus = AssetDisposalStatus.Approved;
        }

        public void Post(Guid id)
        {
            var d = _store.First(x => x.AssetDisposalId == id);
            d.DisposalStatus = AssetDisposalStatus.Posted;
        }

        public void ResetToSeed()
        {
            _store = _seed.Seed();
        }

    }
}
