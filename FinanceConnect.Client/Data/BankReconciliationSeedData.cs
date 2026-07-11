using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class BankReconciliationSeedData
    {
        public static List<BankReconciliationModel> GetSeedData(
            Guid tenantId,
            Guid companyId,
            Guid bankAccountId,
            Guid? branchId)
        {
            var list = new List<BankReconciliationModel>();

            var now = DateTime.UtcNow;

            // ================= RUN 1 — FINALIZED MONTH END =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000001",
                RunType = RunType.MonthEndFinal,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1),
                ToDate = new DateTime(now.Year, now.Month, 1).AddDays(-1),

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,

                ReconciliationStatus = ReconciliationStatus.Finalized,

                // ===== Matching Policy Snapshot =====
                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                // ===== Balances =====
                OpeningBalance_Statement = 250000,
                ClosingBalance_Statement = 278500,

                OpeningBalance_Book = 250000,
                ClosingBalance_Book = 278500,

                DifferenceAmount = 0,
                IsDifferenceWithinTolerance = true,
                ToleranceAmount = 0,

                // ===== Totals =====
                TotalStatementCredits = 95000,
                TotalStatementDebits = 66500,

                TotalBookInflows = 95000,
                TotalBookOutflows = 66500,

                MatchedStatementAmount = 161500,
                MatchedBookAmount = 161500,

                UnmatchedStatementAmount = 0,
                UnmatchedBookAmount = 0,

                MatchedCount = 42,
                UnmatchedStatementCount = 0,
                UnmatchedBookCount = 0,

                UnknownItemCount = 0,
                OutstandingChequeCount = 0,
                DepositInTransitCount = 0,

                // ===== Ownership =====
                PreparedBy = "seed",
                PreparedOn = now.AddDays(-30),

                FinalizedBy = "controller",
                FinalizedOn = now.AddDays(-28),
                FinalizeNotes = "Month-end reconciliation finalized successfully",

                // ===== Audit =====
                CreatedAt = now.AddDays(-30),
                CreatedBy = "seed",
                IsDeleted = false
            });

            // ================= RUN 2 — IN PROGRESS =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000002",
                RunType = RunType.Monthly,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year, now.Month, 1),
                ToDate = now.Date,

                StatementSelectionMode = StatementSelectionModeType.ByStatementFile,

                ReconciliationStatus = ReconciliationStatus.InProgress,

                // ===== Matching Policy Snapshot =====
                AutoMatchEnabled = true,
                DateWindowDays = 3,
                AmountTolerance = 5,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = true,

                // ===== Balances =====
                OpeningBalance_Statement = 278500,
                ClosingBalance_Statement = 295000,

                OpeningBalance_Book = 278500,
                ClosingBalance_Book = 292500,

                DifferenceAmount = 2500,
                IsDifferenceWithinTolerance = false,
                ToleranceAmount = 0,

                // ===== Totals =====
                TotalStatementCredits = 42000,
                TotalStatementDebits = 25500,

                TotalBookInflows = 40000,
                TotalBookOutflows = 25500,

                MatchedStatementAmount = 50000,
                MatchedBookAmount = 50000,

                UnmatchedStatementAmount = 12000,
                UnmatchedBookAmount = 10000,

                MatchedCount = 18,
                UnmatchedStatementCount = 4,
                UnmatchedBookCount = 2,

                UnknownItemCount = 1,
                OutstandingChequeCount = 1,
                DepositInTransitCount = 1,

                // ===== Ownership =====
                PreparedBy = "seed",
                PreparedOn = now.AddDays(-5),

                // ===== Audit =====
                CreatedAt = now.AddDays(-5),
                CreatedBy = "seed",
                IsDeleted = false
            });

            // ================= RUN 3 — DRAFT =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000003",
                RunType = RunType.OnDemand,
                ScopeType = ScopeType.AsOfDate,

                AsOfDate = now.Date,

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,

                ReconciliationStatus = ReconciliationStatus.Draft,

                AutoMatchEnabled = true,
                DateWindowDays = 1,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                PreparedBy = "seed",
                PreparedOn = now,

                CreatedAt = now,
                CreatedBy = "seed",
                IsDeleted = false
            });

            // ================= RUN 4 — FINALIZED QUARTERLY =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000004",
                RunType = RunType.MonthEndFinal,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year, 1, 1),
                ToDate = new DateTime(now.Year, 3, 31),

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Finalized,

                AutoMatchEnabled = true,
                DateWindowDays = 3,
                AmountTolerance = 10,
                AllowManyToManyMatching = true,
                AllowAdjustmentCreationFromRecon = false,

                OpeningBalance_Statement = 180000,
                ClosingBalance_Statement = 215000,
                OpeningBalance_Book = 180000,
                ClosingBalance_Book = 215000,
                DifferenceAmount = 0,
                IsDifferenceWithinTolerance = true,
                ToleranceAmount = 10,

                TotalStatementCredits = 120000,
                TotalStatementDebits = 85000,
                TotalBookInflows = 120000,
                TotalBookOutflows = 85000,

                MatchedStatementAmount = 205000,
                MatchedBookAmount = 205000,
                UnmatchedStatementAmount = 0,
                UnmatchedBookAmount = 0,

                MatchedCount = 56,
                UnmatchedStatementCount = 0,
                UnmatchedBookCount = 0,

                PreparedBy = "accountant",
                PreparedOn = now.AddDays(-60),
                FinalizedBy = "controller",
                FinalizedOn = now.AddDays(-58),
                FinalizeNotes = "Q1 reconciliation completed",

                CreatedAt = now.AddDays(-60),
                CreatedBy = "accountant",
                IsDeleted = false
            });

            // ================= RUN 5 — IN PROGRESS WEEKLY =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000005",
                RunType = RunType.Monthly,
                ScopeType = ScopeType.PeriodRange,

                FromDate = now.AddDays(-14).Date,
                ToDate = now.AddDays(-7).Date,

                StatementSelectionMode = StatementSelectionModeType.ByStatementFile,
                ReconciliationStatus = ReconciliationStatus.InProgress,

                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 5,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = true,

                OpeningBalance_Statement = 310000,
                ClosingBalance_Statement = 325000,
                OpeningBalance_Book = 310000,
                ClosingBalance_Book = 322500,
                DifferenceAmount = 2500,
                IsDifferenceWithinTolerance = false,

                TotalStatementCredits = 45000,
                TotalStatementDebits = 30000,
                TotalBookInflows = 42500,
                TotalBookOutflows = 30000,

                MatchedStatementAmount = 60000,
                MatchedBookAmount = 60000,
                UnmatchedStatementAmount = 15000,
                UnmatchedBookAmount = 12500,

                MatchedCount = 22,
                UnmatchedStatementCount = 3,
                UnmatchedBookCount = 2,
                UnknownItemCount = 1,

                PreparedBy = "junior_accountant",
                PreparedOn = now.AddDays(-3),

                CreatedAt = now.AddDays(-3),
                CreatedBy = "junior_accountant",
                IsDeleted = false
            });

            // ================= RUN 6 — FINALIZED PREVIOUS MONTH =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000006",
                RunType = RunType.MonthEndFinal,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year, now.Month, 1).AddMonths(-2),
                ToDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1).AddDays(-1),

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Finalized,

                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                OpeningBalance_Statement = 200000,
                ClosingBalance_Statement = 250000,
                OpeningBalance_Book = 200000,
                ClosingBalance_Book = 250000,
                DifferenceAmount = 0,
                IsDifferenceWithinTolerance = true,

                TotalStatementCredits = 110000,
                TotalStatementDebits = 60000,
                TotalBookInflows = 110000,
                TotalBookOutflows = 60000,

                MatchedStatementAmount = 170000,
                MatchedBookAmount = 170000,
                UnmatchedStatementAmount = 0,
                UnmatchedBookAmount = 0,

                MatchedCount = 38,
                UnmatchedStatementCount = 0,
                UnmatchedBookCount = 0,

                PreparedBy = "accountant",
                PreparedOn = now.AddDays(-45),
                FinalizedBy = "controller",
                FinalizedOn = now.AddDays(-43),
                FinalizeNotes = "Previous month recon finalized - no exceptions",

                CreatedAt = now.AddDays(-45),
                CreatedBy = "accountant",
                IsDeleted = false
            });

            // ================= RUN 7 — DRAFT ON-DEMAND =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000007",
                RunType = RunType.OnDemand,
                ScopeType = ScopeType.AsOfDate,

                AsOfDate = now.AddDays(-2).Date,

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Draft,

                AutoMatchEnabled = false,
                DateWindowDays = 1,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                PreparedBy = "audit_team",
                PreparedOn = now.AddDays(-2),

                CreatedAt = now.AddDays(-2),
                CreatedBy = "audit_team",
                IsDeleted = false
            });

            // ================= RUN 8 — IN PROGRESS AUTO-MATCH =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000008",
                RunType = RunType.Monthly,
                ScopeType = ScopeType.PeriodRange,

                FromDate = now.AddDays(-21).Date,
                ToDate = now.AddDays(-14).Date,

                StatementSelectionMode = StatementSelectionModeType.ByStatementFile,
                ReconciliationStatus = ReconciliationStatus.InProgress,

                AutoMatchEnabled = true,
                DateWindowDays = 5,
                AmountTolerance = 50,
                AllowManyToManyMatching = true,
                AllowAdjustmentCreationFromRecon = true,

                OpeningBalance_Statement = 420000,
                ClosingBalance_Statement = 445000,
                OpeningBalance_Book = 420000,
                ClosingBalance_Book = 440000,
                DifferenceAmount = 5000,
                IsDifferenceWithinTolerance = false,

                TotalStatementCredits = 80000,
                TotalStatementDebits = 55000,
                TotalBookInflows = 75000,
                TotalBookOutflows = 55000,

                MatchedStatementAmount = 100000,
                MatchedBookAmount = 100000,
                UnmatchedStatementAmount = 35000,
                UnmatchedBookAmount = 20000,

                MatchedCount = 30,
                UnmatchedStatementCount = 5,
                UnmatchedBookCount = 3,
                OutstandingChequeCount = 2,
                DepositInTransitCount = 1,

                PreparedBy = "senior_accountant",
                PreparedOn = now.AddDays(-7),

                CreatedAt = now.AddDays(-7),
                CreatedBy = "senior_accountant",
                IsDeleted = false
            });

            // ================= RUN 9 — FINALIZED YEAR-END =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000009",
                RunType = RunType.MonthEndFinal,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year - 1, 4, 1),
                ToDate = new DateTime(now.Year, 3, 31),

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Finalized,

                AutoMatchEnabled = true,
                DateWindowDays = 3,
                AmountTolerance = 0,
                AllowManyToManyMatching = true,
                AllowAdjustmentCreationFromRecon = false,

                OpeningBalance_Statement = 150000,
                ClosingBalance_Statement = 380000,
                OpeningBalance_Book = 150000,
                ClosingBalance_Book = 380000,
                DifferenceAmount = 0,
                IsDifferenceWithinTolerance = true,

                TotalStatementCredits = 850000,
                TotalStatementDebits = 620000,
                TotalBookInflows = 850000,
                TotalBookOutflows = 620000,

                MatchedStatementAmount = 1470000,
                MatchedBookAmount = 1470000,
                UnmatchedStatementAmount = 0,
                UnmatchedBookAmount = 0,

                MatchedCount = 312,
                UnmatchedStatementCount = 0,
                UnmatchedBookCount = 0,

                PreparedBy = "finance_manager",
                PreparedOn = now.AddDays(-90),
                FinalizedBy = "cfo",
                FinalizedOn = now.AddDays(-88),
                FinalizeNotes = "Annual year-end reconciliation - auditor verified",

                CreatedAt = now.AddDays(-90),
                CreatedBy = "finance_manager",
                IsDeleted = false
            });

            // ================= RUN 10 — DRAFT MID-MONTH =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000010",
                RunType = RunType.OnDemand,
                ScopeType = ScopeType.PeriodRange,

                FromDate = new DateTime(now.Year, now.Month, 1),
                ToDate = new DateTime(now.Year, now.Month, 15),

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Draft,

                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                PreparedBy = "accountant",
                PreparedOn = now.AddDays(-1),

                CreatedAt = now.AddDays(-1),
                CreatedBy = "accountant",
                IsDeleted = false
            });

            // ================= RUN 11 — IN PROGRESS WITH EXCEPTIONS =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000011",
                RunType = RunType.Monthly,
                ScopeType = ScopeType.PeriodRange,

                FromDate = now.AddDays(-28).Date,
                ToDate = now.AddDays(-21).Date,

                StatementSelectionMode = StatementSelectionModeType.ByStatementFile,
                ReconciliationStatus = ReconciliationStatus.InProgress,

                AutoMatchEnabled = true,
                DateWindowDays = 3,
                AmountTolerance = 25,
                AllowManyToManyMatching = true,
                AllowAdjustmentCreationFromRecon = true,

                OpeningBalance_Statement = 350000,
                ClosingBalance_Statement = 380000,
                OpeningBalance_Book = 350000,
                ClosingBalance_Book = 375000,
                DifferenceAmount = 5000,
                IsDifferenceWithinTolerance = false,

                TotalStatementCredits = 90000,
                TotalStatementDebits = 60000,
                TotalBookInflows = 85000,
                TotalBookOutflows = 60000,

                MatchedStatementAmount = 120000,
                MatchedBookAmount = 120000,
                UnmatchedStatementAmount = 30000,
                UnmatchedBookAmount = 25000,

                MatchedCount = 28,
                UnmatchedStatementCount = 6,
                UnmatchedBookCount = 4,
                UnknownItemCount = 2,
                OutstandingChequeCount = 3,

                PreparedBy = "recon_specialist",
                PreparedOn = now.AddDays(-10),

                CreatedAt = now.AddDays(-10),
                CreatedBy = "recon_specialist",
                IsDeleted = false
            });

            // ================= RUN 12 — FINALIZED LAST WEEK =================
            list.Add(new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),
                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = "BNKREC-000012",
                RunType = RunType.MonthEndFinal,
                ScopeType = ScopeType.PeriodRange,

                FromDate = now.AddDays(-35).Date,
                ToDate = now.AddDays(-28).Date,

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,
                ReconciliationStatus = ReconciliationStatus.Finalized,

                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                OpeningBalance_Statement = 295000,
                ClosingBalance_Statement = 310000,
                OpeningBalance_Book = 295000,
                ClosingBalance_Book = 310000,
                DifferenceAmount = 0,
                IsDifferenceWithinTolerance = true,

                TotalStatementCredits = 55000,
                TotalStatementDebits = 40000,
                TotalBookInflows = 55000,
                TotalBookOutflows = 40000,

                MatchedStatementAmount = 95000,
                MatchedBookAmount = 95000,
                UnmatchedStatementAmount = 0,
                UnmatchedBookAmount = 0,

                MatchedCount = 24,
                UnmatchedStatementCount = 0,
                UnmatchedBookCount = 0,

                PreparedBy = "accountant",
                PreparedOn = now.AddDays(-15),
                FinalizedBy = "controller",
                FinalizedOn = now.AddDays(-14),
                FinalizeNotes = "Weekly recon completed - all items matched",

                CreatedAt = now.AddDays(-15),
                CreatedBy = "accountant",
                IsDeleted = false
            });

            return list;
        }
    }
}
