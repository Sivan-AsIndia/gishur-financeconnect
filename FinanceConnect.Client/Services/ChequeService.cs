using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class ChequeService
    {
        private readonly List<ChequeModel> _cheques = new();
        public ChequeService()
        {
            _cheques = ChequeServiceData.Get();
        }
        public List<ChequeModel> GetAll() => _cheques;

        public ChequeModel? GetById(Guid id)
            => _cheques.FirstOrDefault(x => x.Id == id);

        public void Add(ChequeModel model)
        {
            model.Status = ChequeStatus.Draft;
            _cheques.Add(model);
        }

        public void Update(ChequeModel model)
        {
            var c = GetById(model.Id);
            if (c == null) return;

            // Core
            c.BranchId = model.BranchId;
            c.Branch = model.Branch;
            c.Direction = model.Direction;
            c.Status = model.Status;

            // Editable fields (lock after Printed in real app)
            c.ChequeNumber = model.ChequeNumber;
            c.Amount = model.Amount;
            c.ChequeDate = model.ChequeDate;
            c.Currency = model.Currency;
            c.PayeeName = model.PayeeName;
            c.CrossingType = model.CrossingType;

            // Counterparty
            c.CounterpartyType = model.CounterpartyType;
            c.CounterpartyName = model.CounterpartyName;
            c.CounterpartyContact = model.CounterpartyContact;

            // Bank
            c.OurBankAccountId = model.OurBankAccountId;
            c.OurBankAccount = model.OurBankAccount;
            c.DrawerBankName = model.DrawerBankName;
            c.DrawerBankBranch = model.DrawerBankBranch;
            c.DrawerAccountMasked = model.DrawerAccountMasked;
            c.MICR_IFSC = model.MICR_IFSC;
            c.IsCTSCompliant = model.IsCTSCompliant;

            // Linkage
            c.SourceModule = model.SourceModule;
            c.SourceDocumentType = model.SourceDocumentType;
            c.SourceDocumentNo = model.SourceDocumentNo;

            // Bounce
            c.BounceReason = model.BounceReason;
            c.BounceReasonText = model.BounceReasonText;
            c.BounceCharges = model.BounceCharges;

            // Notes
            c.Narration = model.Narration;

            // Lifecycle dates
            c.PreparedOn = model.PreparedOn;
            c.PrintedOn = model.PrintedOn;
            c.IssuedOrReceivedOn = model.IssuedOrReceivedOn;
            c.IssuedOn = model.IssuedOn;
            c.ReceivedOn = model.ReceivedOn;
            c.DepositedOn = model.DepositedOn;
            c.PresentedOn = model.PresentedOn;
            c.ClearedOn = model.ClearedOn;
            c.BouncedOn = model.BouncedOn;
            c.StoppedOn = model.StoppedOn;
            c.CancelledOn = model.CancelledOn;
            c.StaleOn = model.StaleOn;
            c.ExpectedClearBy = model.ExpectedClearBy;

            c.UpdatedAt = DateTime.Now;
        }

        // ===== WORKFLOW =====

        public void MarkPrinted(Guid id)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Printed;
            c.PrintedOn = DateTime.Now;
        }

        public void MarkIssued(Guid id)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Issued;
            c.IssuedOn = DateTime.Now;
        }

        public void MarkReceived(Guid id)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Received;
            c.ReceivedOn = DateTime.Now;
        }

        public void MarkDeposited(Guid id)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Deposited;
            c.DepositedOn = DateTime.Now;
        }

        public void MarkCleared(Guid id)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Cleared;
            c.ClearedOn = DateTime.Now;
        }

        public void MarkBounced(Guid id, string reason)
        {
            var c = GetById(id);
            if (c == null) return;

            c.Status = ChequeStatus.Bounced;
            c.BounceReason = reason;
            c.BouncedOn = DateTime.Now;
        }
    }
}
