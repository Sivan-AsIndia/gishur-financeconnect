using FinanceConnect.Client.Data;
using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceConnect.Client.Services
{
    public class FinancialTransactionService
    {
        private static List<FinancialTransactionModel> _store = new();
        private readonly List<FinancialTransactionModel> _seedTransaction = new();
        //private readonly List<TransactionLineModel> _lines = new();

        private readonly List<CompanyModel> _companies = new();
        private readonly List<BranchModel> _branches = new();
        private readonly List<TransactionTypeModel> _types = new();
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private readonly TransactionTypeService _typeService;
        private readonly FinancialTransactionSeedData _seed;
        private readonly FiscalYearService _fiscalYearService;
        private readonly AccountingPeriodService _accPeriodService;
        private readonly DocumentNumberSeriesService _docNumSeriesService;
        private readonly TransactionStatusService _transactionStatusService;
        public FinancialTransactionService(MasterDataService masterDataService, BranchService branchService,
            TransactionTypeService typeService, FiscalYearService fiscalYearService,
            AccountingPeriodService accPeriodService, DocumentNumberSeriesService docNumSeriesService, TransactionStatusService transactionStatusService)
        {
            _masterDataService = masterDataService;
            _branchService = branchService;
            _typeService = typeService;
            _fiscalYearService = fiscalYearService;
            _accPeriodService = accPeriodService;
            _docNumSeriesService = docNumSeriesService;
            _transactionStatusService = transactionStatusService;
            _seed = new FinancialTransactionSeedData(docNumSeriesService, fiscalYearService, accPeriodService, transactionStatusService);
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _branches = _branchService.GetAll().Where(t => t.Status == "Active").ToList();
            _types = _typeService
                .GetAll()
                .Where(t => t.IsActive)
                .ToList();
            _seedTransaction = SeedData();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _store = CloneList(_seedTransaction);
        }

        public List<CompanyModel> GetCompanies() =>
             _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

        public List<BranchModel> GetBranchesByCompany(Guid? companyId) =>
            _branchService.GetAll()
                .Where(b => b.CompanyId == companyId && b.Status == "Active")
                .OrderBy(b => b.BranchName)
                .ToList();

        public List<TransactionTypeModel> GetTransactionTypesByCompany(Guid? companyId) =>
            _typeService
                .GetAll()
                .Where(t => t.IsActive && t.CompanyId == companyId).OrderBy(t => t.Name).ToList();
        

        public List<FinancialTransactionModel> GetAll()
        {
            return _store
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        public FinancialTransactionModel? GetById(Guid id)
        {
            return _store.FirstOrDefault(x => x.FinancialTransactionId == id);
        }

        public void Create(FinancialTransactionModel model,TransactionTypeModel type)
        {
            ValidateCreate(model);
            model.DocumentNo = GenerateDocumentNumber(
            type,
            model.TransactionDate!.Value,
            model.CompanyId!.Value,
            model.BranchId);
            model.FinancialTransactionId = Guid.NewGuid();
            model.TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            model.CreatedAt = DateTime.UtcNow;
            model.Status = "Draft";

            _store.Add(model);
        }

        public string GenerateDocumentNumber(
        TransactionTypeModel type,
        DateTime TransactionDate,
        Guid companyId,
        Guid? branchId)
        {
            if (!type.DocumentNumberSeriesId.HasValue)
                throw new InvalidOperationException("Journal number series not configured.");

            // ⭐ Delegate to central numbering engine
            return _docNumSeriesService.Generate(
                type.DocumentNumberSeriesId.Value,
                TransactionDate,
                companyId,
                branchId);
        }

        public void Update(FinancialTransactionModel model)
        {
            var existing = GetById(model.FinancialTransactionId);
            if (existing == null) return;

            if (existing.Status != "Draft")
                throw new InvalidOperationException("Only Draft transactions can be edited.");

            ValidateHeader(model);

            model.UpdatedAt = DateTime.UtcNow;
            _store[_store.IndexOf(existing)] = model;
        }

        public void Submit(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.Status != "Draft")
                throw new InvalidOperationException("Only Draft transactions can be submitted.");

            //ValidateBeforeSubmit(tx);

            tx.Status = "Submitted";
            tx.SubmittedAt = DateTime.UtcNow;
        }

        public void Approve(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.Status != "Submitted")
                throw new InvalidOperationException("Only Submitted transactions can be approved.");

            tx.Status = "Approved";
            tx.ApprovedAt = DateTime.UtcNow;
        }

        public void Post(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.Status != "Approved")
                throw new InvalidOperationException("Only Approved transactions can be posted.");

            ValidateBeforePost(tx);

            tx.Status = "Posted";
            tx.PostedAt = DateTime.UtcNow;
        }

        public void Cancel(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.Status != "Draft")
                throw new InvalidOperationException("Only Draft transactions can be cancelled.");

            tx.Status = "Cancelled";
        }

        public void Delete(Guid id)
        {
            var tx = GetById(id);
            if (tx == null) return;

            if (tx.Status != "Draft")
                throw new InvalidOperationException("Only Draft transactions can be deleted.");

            _store.Remove(tx);
            //_lines.RemoveAll(l => l.FinancialTransactionId == id);
        }

        public FinancialTransactionModel Reverse(Guid id)
        {
            var original = GetById(id);
            if (original == null)
                throw new InvalidOperationException("Transaction not found.");

            if (original.Status != "Posted")
                throw new InvalidOperationException("Only Posted transactions can be reversed.");

            var reversal = new FinancialTransactionModel
            {
                FinancialTransactionId = Guid.NewGuid(),
                TenantId = original.TenantId,

                CompanyId = original.CompanyId,
                BranchId = original.BranchId,

                TransactionTypeId = original.TransactionTypeId,
                TransactionTypeName = original.TransactionTypeName,

                DocumentNo = $"REV-{original.DocumentNo}",
                SourceModule = original.SourceModule,
                SourceDocumentId = original.SourceDocumentId,
                SourceDocumentNo = original.SourceDocumentNo,

                TransactionDate = DateTime.UtcNow,
                AccountingPeriod = ResolveAccountingPeriod(DateTime.UtcNow),

                CurrencyId = original.CurrencyId,
                TransactionAmount = -original.TransactionAmount,
                BaseAmount = -original.BaseAmount,

                Status = "Draft",
                IsAutoGenerated = true,
                IsReversal = true,
                ReversalOfTransactionId = original.FinancialTransactionId,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            _store.Add(reversal);

            // Reverse lines
            //var originalLines = GetLines(original.FinancialTransactionId);
            //foreach (var line in originalLines)
            //{
            //    _lines.Add(new TransactionLineModel
            //    {
            //        TransactionLineId = Guid.NewGuid(),
            //        FinancialTransactionId = reversal.FinancialTransactionId,
            //        LineType = line.LineType,
            //        Description = $"REVERSAL - {line.Description}",
            //        LineAmount = -line.LineAmount,
            //        BaseAmount = -line.BaseAmount
            //    });
            //}

            return reversal;
        }

        // ==============================
        // TRANSACTION LINES (#18)
        // ==============================
        //public List<TransactionLineModel> GetLines(Guid txId)
        //{
        //    return _lines
        //        .Where(l => l.FinancialTransactionId == txId)
        //        .ToList();
        //}

        //public void AddLine(TransactionLineModel line)
        //{
        //    var tx = GetById(line.FinancialTransactionId);
        //    if (tx == null)
        //        throw new InvalidOperationException("Transaction not found.");

        //    if (tx.Status != "Draft")
        //        throw new InvalidOperationException("Lines can only be edited in Draft.");

        //    line.TransactionLineId = Guid.NewGuid();
        //    _lines.Add(line);

        //    SyncTotals(tx.FinancialTransactionId);
        //}

        //public void RemoveLine(Guid lineId)
        //{
        //    var line = _lines.FirstOrDefault(l => l.TransactionLineId == lineId);
        //    if (line == null) return;

        //    var tx = GetById(line.FinancialTransactionId);
        //    if (tx == null || tx.Status != "Draft")
        //        throw new InvalidOperationException("Cannot modify lines after submit.");

        //    _lines.Remove(line);
        //    SyncTotals(tx.FinancialTransactionId);
        //}

        // ==============================
        // VALIDATION & HELPERS
        // ==============================
        void ValidateCreate(FinancialTransactionModel model)
        {
            ValidateHeader(model);

            bool duplicateSource = _store.Any(x =>
                x.TenantId == model.TenantId &&
                x.CompanyId == model.CompanyId &&
                x.SourceModule == model.SourceModule &&
                x.SourceDocumentId == model.SourceDocumentId);

            if (duplicateSource)
                throw new InvalidOperationException(
                    "Duplicate transaction detected for same source document.");
        }

        void ValidateHeader(FinancialTransactionModel model)
        {
            if (model.CompanyId == Guid.Empty)
                throw new InvalidOperationException("Company is mandatory.");

            if (model.BranchId == Guid.Empty)
                throw new InvalidOperationException("Branch is mandatory.");

            if (model.TransactionTypeId == Guid.Empty)
                throw new InvalidOperationException("Transaction Type is required.");

            if (model.TransactionDate == default)
                throw new InvalidOperationException("Transaction Date is required.");

            if (!Enum.IsDefined(typeof(SourceModule), model.SourceModule))
                throw new InvalidOperationException("Invalid Source Module selected.");

            if (model.TransactionAmount <= 0)
                throw new InvalidOperationException("Transaction Amount must be greater than zero.");
        }

        //void ValidateBeforeSubmit(FinancialTransactionModel tx)
        //{
        //    var lines = GetLines(tx.FinancialTransactionId);

        //    if (!lines.Any())
        //        throw new InvalidOperationException("Transaction must have at least one line.");

        //    decimal sum = lines.Sum(l => l.LineAmount);
        //    if (sum != tx.TransactionAmount)
        //        throw new InvalidOperationException(
        //            "Transaction lines total does not match header amount.");
        //}

        void ValidateBeforePost(FinancialTransactionModel tx)
        {
            if (string.IsNullOrWhiteSpace(tx.DocumentNo))
                throw new InvalidOperationException("Document Number must be assigned before posting.");

            // Example period lock logic
            if (tx.AccountingPeriod.EndsWith("CLOSED"))
                throw new InvalidOperationException("Cannot post into closed accounting period.");
        }

        //void SyncTotals(Guid txId)
        //{
        //    var tx = GetById(txId);
        //    if (tx == null) return;

        //    var lines = GetLines(txId);

        //    tx.TransactionAmount = lines.Sum(l => l.LineAmount);
        //    tx.BaseAmount = lines.Sum(l => l.BaseAmount);
        //    tx.UpdatedAt = DateTime.UtcNow;
        //}

        public string ResolveAccountingPeriod(DateTime date)
        {
            return $"{date:yyyy-MM}";
        }

        public AccountingPeriodModel GetAccountingPeriodByDate(FinancialTransactionModel tx)
        {
            var fiscalYear = _fiscalYearService.GetAll().FirstOrDefault(fy =>
                tx.TransactionDate >= fy.StartDate &&
                tx.TransactionDate <= fy.EndDate &&
                fy.CompanyId == tx.CompanyId &&
                fy.Status == FiscalYearStatus.Open);

            if (fiscalYear == null)
                throw new InvalidOperationException("No active fiscal year found for this company/date.");

            var accountingPeriod = GetAccPeriodByDate(tx, fiscalYear);

            if (accountingPeriod == null)
                throw new InvalidOperationException("No open accounting period found for this date.");

            return accountingPeriod;
        }

        public AccountingPeriodModel? GetAccPeriodByDate(FinancialTransactionModel tx, FiscalYearModel fiscalYear)
        {
            var AccountingPeriod = _accPeriodService.GetAll().FirstOrDefault(acc =>
                 acc.FiscalYearId == fiscalYear.Id &&
                tx.TransactionDate >= acc.StartDate &&
                 tx.TransactionDate <= acc.EndDate && acc.CompanyId == tx.CompanyId && acc.Status == AccountingPeriodStatus.Open);


            return AccountingPeriod;
        }


        private List<FinancialTransactionModel> SeedData()
        {
            var seeded = _seed.Seed(_companies, _branches, _types);
            return seeded;
        }
    }
}
