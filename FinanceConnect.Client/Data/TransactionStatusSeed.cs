using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class TransactionStatusSeed
    {
        public void Seed(Guid tenantId, List<TransactionStatusModel> store)
        {
            if (store.Any(x => x.TenantId == tenantId))
                return;

            Add(store, tenantId, "DRAFT", "Draft", StageCategory.DraftStage,
                edit: true, submit: true, cancel: true,
                badgeTone: BadgeTone.Neutral);

            Add(store, tenantId, "SUBMITTED", "Submitted", StageCategory.ApprovalStage,
                approve: true,
                badgeTone: BadgeTone.Warning);

            Add(store, tenantId, "APPROVED", "Approved", StageCategory.PostingStage,
                post: true,
                badgeTone: BadgeTone.Success);

            Add(store, tenantId, "REJECTED", "Rejected", StageCategory.DraftStage,
                edit: true, submit: true, cancel: true,
                badgeTone: BadgeTone.Danger);

            Add(store, tenantId, "POSTED", "Posted", StageCategory.FinalStage,
                reverse: true, isFinal: true,
                badgeTone: BadgeTone.Success);

            Add(store, tenantId, "REVERSED", "Reversed", StageCategory.FinalStage,
                isFinal: true,
                badgeTone: BadgeTone.Warning);

            Add(store, tenantId, "CANCELLED", "Cancelled", StageCategory.FinalStage,
                isFinal: true,
                badgeTone: BadgeTone.Danger);
        }

        private void Add(
            List<TransactionStatusModel> store,
            Guid tenantId,
            string code,
            string name,
            StageCategory stage,
            bool edit = false,
            bool submit = false,
            bool approve = false,
            bool post = false,
            bool reverse = false,
            bool cancel = false,
            bool isFinal = false,
            BadgeTone badgeTone = BadgeTone.Neutral)
        {
            store.Add(new TransactionStatusModel
            {
                TenantId = tenantId,
                CompanyScopeMode = CompanyScopeMode.Global,

                Code = code,
                Name = name,
                StageCategory = stage,

                AllowHeaderEdit = edit,
                AllowLineEdit = edit,
                AllowDelete = edit,
                AllowSubmit = submit,
                AllowApproveReject = approve,
                AllowPost = post,
                AllowReverse = reverse,
                AllowCancel = cancel,

                IsFinal = isFinal,

                // ✅ UI Badge
                BadgeLabel = name,
                BadgeTone = badgeTone,

                IsActive = true,
                IsSystemDefined = true,

                DisplayOrder = store.Count + 1,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
