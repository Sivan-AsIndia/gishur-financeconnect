using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class CostAllocationSeedData
    {
        // ── Lookup name maps ───────────────────────────────────────────────────

        public static readonly Dictionary<Guid, string> CostCenterNames = new()
        {
            { Guid.Parse("aa000001-0000-0000-0000-000000000001"), "Administration" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000002"), "Delivery & Logistics" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000003"), "Sales & Marketing" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000004"), "Human Resources" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000005"), "Information Technology" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000006"), "Finance & Accounts" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000007"), "Production / Factory" },
            { Guid.Parse("aa000001-0000-0000-0000-000000000099"), "Head Office – Shared Pool" },
        };

        private static readonly Guid CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid FY2026 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid UserFPA = Guid.Parse("ffffffff-0000-0000-0000-000000000001");
        private static readonly Guid UserCtrl = Guid.Parse("ffffffff-0000-0000-0000-000000000002");

        private static readonly Guid CC_Admin = Guid.Parse("aa000001-0000-0000-0000-000000000001");
        private static readonly Guid CC_Deliv = Guid.Parse("aa000001-0000-0000-0000-000000000002");
        private static readonly Guid CC_Sales = Guid.Parse("aa000001-0000-0000-0000-000000000003");
        private static readonly Guid CC_HR = Guid.Parse("aa000001-0000-0000-0000-000000000004");
        private static readonly Guid CC_IT = Guid.Parse("aa000001-0000-0000-0000-000000000005");
        private static readonly Guid CC_Fin = Guid.Parse("aa000001-0000-0000-0000-000000000006");
        private static readonly Guid CC_Prod = Guid.Parse("aa000001-0000-0000-0000-000000000007");
        private static readonly Guid CC_HO = Guid.Parse("aa000001-0000-0000-0000-000000000099");

        // ── Seed records ───────────────────────────────────────────────────────

        public static List<CostAllocationViewModel> GetAll()
        {
            return new List<CostAllocationViewModel>
            {
                // ── ALLOC-2026-001 : Rent (Approved) ──────────────────────────
                new()
                {
                    CostAllocationId       = Guid.Parse("77000001-0000-0000-0000-000000000001"),
                    TenantId               = TenantId,
                    CompanyId              = CompanyId,
                    AllocationCode         = "ALLOC-2026-001",
                    AllocationName         = "Head Office Rent Allocation – Apr 2026",
                    Description            = "Monthly allocation of HO rent expense across all cost centers based on fixed percentage agreed in FY2026 policy document.",
                    AllocationType         = AllocationType.OverheadDistribution,
                    AllocationStatus       = AllocationStatus.Approved,
                    AllocationDate         = new DateTime(2026, 4, 1),
                    EffectiveDate          = new DateTime(2026, 4, 30),
                    FiscalYearId           = FY2026,
                    ScopeType              = ScopeTypecost.Company,
                    SourceCostCenterId     = CC_HO,
                    SourceAmount           = 100000,
                    SourceAmountType       = SourceAmountType.Budgeted,
                    SourceReferenceText    = "HO Rent Apr 2026",
                    AllocationMethod       = AllocationMethod.FixedPercentage,
                    AllocationBasisType    = AllocationBasisType.Static,
                    RoundingRule           = RoundingRule.ResidualToLastLine,
                    MustFullyAllocateSource = true,
                    IsManualOverrideAllowed = false,
                    TotalTargetCount       = 4,
                    TotalAllocatedAmount   = 100000,
                    UnallocatedAmount      = 0,
                    IsFullyAllocated       = true,
                    PreparedByUserId       = UserFPA,
                    SubmittedByUserId      = UserFPA,
                    ApprovedByUserId       = UserCtrl,
                    SubmittedOn            = new DateTime(2026, 4, 2),
                    ApprovedOn             = new DateTime(2026, 4, 3),
                    AllocationAssumptionText = "Fixed percentages per FY2026 cost allocation policy. Approved by CFO in budget circular dated Jan 2026.",
                    AttachmentCount        = 2,
                    CreatedAt              = new DateTime(2026, 4, 1, 9, 0, 0),
                    CreatedBy              = UserFPA,
                    Lines = new()
                    {
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 10, TargetCostCenterId = CC_Admin, TargetCostCenterName = "Administration",       AllocationPercent = 30, AllocatedAmount = 30000, AllocationLineStatus = AllocationLineStatus.Approved },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 20, TargetCostCenterId = CC_Deliv, TargetCostCenterName = "Delivery & Logistics", AllocationPercent = 40, AllocatedAmount = 40000, AllocationLineStatus = AllocationLineStatus.Approved },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 30, TargetCostCenterId = CC_Sales, TargetCostCenterName = "Sales & Marketing",    AllocationPercent = 20, AllocatedAmount = 20000, AllocationLineStatus = AllocationLineStatus.Approved },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 40, TargetCostCenterId = CC_HR,    TargetCostCenterName = "Human Resources",      AllocationPercent = 10, AllocatedAmount = 10000, AllocationLineStatus = AllocationLineStatus.Approved },
                    }
                },

                // ── ALLOC-2026-002 : HR Shared Services (Submitted) ───────────
                new()
                {
                    CostAllocationId       = Guid.Parse("77000001-0000-0000-0000-000000000002"),
                    TenantId               = TenantId,
                    CompanyId              = CompanyId,
                    AllocationCode         = "ALLOC-2026-002",
                    AllocationName         = "Shared HR Cost Pool – Apr 2026",
                    Description            = "Allocation of shared HR service costs based on employee headcount per cost center.",
                    AllocationType         = AllocationType.ActualAllocation,
                    AllocationStatus       = AllocationStatus.Submitted,
                    AllocationDate         = new DateTime(2026, 4, 5),
                    EffectiveDate          = new DateTime(2026, 4, 30),
                    FiscalYearId           = FY2026,
                    ScopeType              = ScopeTypecost.Company,
                    SourceCostCenterId     = CC_HR,
                    SourceAmount           = 250000,
                    SourceAmountType       = SourceAmountType.Actual,
                    SourceReferenceText    = "HR Shared Service Cost Pool – Apr 2026",
                    AllocationMethod       = AllocationMethod.HeadcountBased,
                    AllocationBasisType    = AllocationBasisType.Dynamic,
                    DriverReferenceCode    = "HEADCOUNT",
                    DriverAsOfDate         = new DateTime(2026, 4, 1),
                    RoundingRule           = RoundingRule.ResidualToLastLine,
                    MustFullyAllocateSource = true,
                    IsManualOverrideAllowed = true,
                    TotalTargetCount       = 5,
                    TotalAllocatedAmount   = 250000,
                    UnallocatedAmount      = 0,
                    IsFullyAllocated       = true,
                    PreparedByUserId       = UserFPA,
                    SubmittedByUserId      = UserFPA,
                    SubmittedOn            = new DateTime(2026, 4, 6),
                    AllocationAssumptionText = "Headcount snapshot as of 01-Apr-2026 from HR system. Includes all active employees; excludes contractors.",
                    AttachmentCount        = 1,
                    CreatedAt              = new DateTime(2026, 4, 5, 10, 30, 0),
                    CreatedBy              = UserFPA,
                    Lines = new()
                    {
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 10, TargetCostCenterId = CC_Admin, TargetCostCenterName = "Administration",       BasisValue = 15, AllocationPercent = 15,   AllocatedAmount = 37500,  AllocationLineStatus = AllocationLineStatus.Calculated },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 20, TargetCostCenterId = CC_Deliv, TargetCostCenterName = "Delivery & Logistics", BasisValue = 30, AllocationPercent = 30,   AllocatedAmount = 75000,  AllocationLineStatus = AllocationLineStatus.Calculated },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 30, TargetCostCenterId = CC_Sales, TargetCostCenterName = "Sales & Marketing",    BasisValue = 20, AllocationPercent = 20,   AllocatedAmount = 50000,  AllocationLineStatus = AllocationLineStatus.Calculated },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 40, TargetCostCenterId = CC_Prod,  TargetCostCenterName = "Production / Factory", BasisValue = 25, AllocationPercent = 25,   AllocatedAmount = 62500,  AllocationLineStatus = AllocationLineStatus.Calculated },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 50, TargetCostCenterId = CC_Fin,   TargetCostCenterName = "Finance & Accounts",   BasisValue = 10, AllocationPercent = 10,   AllocatedAmount = 25000,  AllocationLineStatus = AllocationLineStatus.Calculated, ManualOverrideFlag = true, ManualOverrideReason = "Rounded up to nearest 1000 per policy" },
                    }
                },

                // ── ALLOC-2026-003 : IT License (Draft) ───────────────────────
                new()
                {
                    CostAllocationId       = Guid.Parse("77000001-0000-0000-0000-000000000003"),
                    TenantId               = TenantId,
                    CompanyId              = CompanyId,
                    AllocationCode         = "ALLOC-2026-003",
                    AllocationName         = "Enterprise SaaS License Budget Distribution – Q1 FY2026",
                    Description            = "Distribution of enterprise software license cost to consuming cost centers based on active user counts.",
                    AllocationType         = AllocationType.BudgetAllocation,
                    AllocationStatus       = AllocationStatus.Draft,
                    AllocationDate         = new DateTime(2026, 4, 10),
                    EffectiveDate          = new DateTime(2026, 6, 30),
                    FiscalYearId           = FY2026,
                    ScopeType              = ScopeTypecost.Company,
                    SourceCostCenterId     = CC_IT,
                    SourceAmount           = 180000,
                    SourceAmountType       = SourceAmountType.Budgeted,
                    SourceReferenceText    = "Enterprise SaaS License – ERP & CRM Annual",
                    AllocationMethod       = AllocationMethod.UsageBased,
                    AllocationBasisType    = AllocationBasisType.ImportedDriver,
                    DriverReferenceCode    = "ACTIVE_USERS",
                    DriverAsOfDate         = new DateTime(2026, 4, 1),
                    RoundingRule           = RoundingRule.ResidualToLastLine,
                    MustFullyAllocateSource = true,
                    IsManualOverrideAllowed = true,
                    TotalTargetCount       = 4,
                    TotalAllocatedAmount   = 144000,
                    UnallocatedAmount      = 36000,
                    IsFullyAllocated       = false,
                    PreparedByUserId       = UserFPA,
                    AttachmentCount        = 0,
                    CreatedAt              = new DateTime(2026, 4, 10, 11, 0, 0),
                    CreatedBy              = UserFPA,
                    Lines = new()
                    {
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 10, TargetCostCenterId = CC_Sales, TargetCostCenterName = "Sales & Marketing",    BasisValue = 45, AllocatedAmount = 50000, AllocationLineStatus = AllocationLineStatus.Draft },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 20, TargetCostCenterId = CC_Fin,   TargetCostCenterName = "Finance & Accounts",   BasisValue = 30, AllocatedAmount = 35000, AllocationLineStatus = AllocationLineStatus.Draft },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 30, TargetCostCenterId = CC_Admin, TargetCostCenterName = "Administration",       BasisValue = 20, AllocatedAmount = 34000, AllocationLineStatus = AllocationLineStatus.Draft },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 40, TargetCostCenterId = CC_HR,    TargetCostCenterName = "Human Resources",      BasisValue = 10, AllocatedAmount = 25000, AllocationLineStatus = AllocationLineStatus.Draft },
                    }
                },

                // ── ALLOC-2026-004 : Electricity (Locked) ─────────────────────
                new()
                {
                    CostAllocationId       = Guid.Parse("77000001-0000-0000-0000-000000000004"),
                    TenantId               = TenantId,
                    CompanyId              = CompanyId,
                    AllocationCode         = "ALLOC-2026-003",
                    AllocationName         = "Factory Electricity – Mar 2026",
                    Description            = "Actual electricity cost for March 2026 allocated to Production and support functions by floor area.",
                    AllocationType         = AllocationType.ActualAllocation,
                    AllocationStatus       = AllocationStatus.Locked,
                    AllocationDate         = new DateTime(2026, 3, 31),
                    EffectiveDate          = new DateTime(2026, 3, 31),
                    FiscalYearId           = FY2026,
                    ScopeType              = ScopeTypecost.Company,
                    SourceCostCenterId     = CC_HO,
                    SourceAmount           = 75000,
                    SourceAmountType       = SourceAmountType.Actual,
                    SourceReferenceText    = "EB Bill Mar 2026 – TNEB Invoice #TN-2026-03-0981",
                    AllocationMethod       = AllocationMethod.FloorAreaBased,
                    AllocationBasisType    = AllocationBasisType.Static,
                    DriverReferenceCode    = "FLOOR_AREA",
                    RoundingRule           = RoundingRule.ResidualToLastLine,
                    MustFullyAllocateSource = true,
                    IsManualOverrideAllowed = false,
                    TotalTargetCount       = 3,
                    TotalAllocatedAmount   = 75000,
                    UnallocatedAmount      = 0,
                    IsFullyAllocated       = true,
                    IsLocked               = true,
                    PreparedByUserId       = UserFPA,
                    SubmittedByUserId      = UserFPA,
                    ApprovedByUserId       = UserCtrl,
                    SubmittedOn            = new DateTime(2026, 3, 31),
                    ApprovedOn             = new DateTime(2026, 4, 1),
                    LockedOn               = new DateTime(2026, 4, 2),
                    LockedBy               = UserCtrl,
                    AllocationAssumptionText = "Floor area data from Facilities Management report Q1-FY2026. Production 60%, Admin 25%, HR 15%.",
                    AttachmentCount        = 3,
                    CreatedAt              = new DateTime(2026, 3, 31, 8, 0, 0),
                    CreatedBy              = UserFPA,
                    Lines = new()
                    {
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 10, TargetCostCenterId = CC_Prod,  TargetCostCenterName = "Production / Factory", BasisValue = 6000, AllocationPercent = 60, AllocatedAmount = 45000, AllocationLineStatus = AllocationLineStatus.Locked },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 20, TargetCostCenterId = CC_Admin, TargetCostCenterName = "Administration",       BasisValue = 2500, AllocationPercent = 25, AllocatedAmount = 18750, AllocationLineStatus = AllocationLineStatus.Locked },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 30, TargetCostCenterId = CC_HR,    TargetCostCenterName = "Human Resources",      BasisValue = 1500, AllocationPercent = 15, AllocatedAmount = 11250, AllocationLineStatus = AllocationLineStatus.Locked },
                    }
                },

                // ── ALLOC-2026-005 : Transport Pool (Reversed) ────────────────
                new()
                {
                    CostAllocationId       = Guid.Parse("77000001-0000-0000-0000-000000000005"),
                    TenantId               = TenantId,
                    CompanyId              = CompanyId,
                    AllocationCode         = "ALLOC-2026-005",
                    AllocationName         = "Transport Pool – Feb 2026 (Reversed)",
                    Description            = "Transport cost allocation for February 2026. Reversed due to incorrect driver data.",
                    AllocationType         = AllocationType.ActualAllocation,
                    AllocationStatus       = AllocationStatus.Reversed,
                    AllocationDate         = new DateTime(2026, 2, 28),
                    EffectiveDate          = new DateTime(2026, 2, 28),
                    FiscalYearId           = FY2026,
                    ScopeType              = ScopeTypecost.Company,
                    SourceCostCenterId     = CC_Deliv,
                    SourceAmount           = 60000,
                    SourceAmountType       = SourceAmountType.Actual,
                    SourceReferenceText    = "Transport Vendor Invoice – Feb 2026",
                    AllocationMethod       = AllocationMethod.RevenueBased,
                    AllocationBasisType    = AllocationBasisType.Dynamic,
                    DriverReferenceCode    = "REVENUE_SHARE",
                    RoundingRule           = RoundingRule.ResidualToLastLine,
                    MustFullyAllocateSource = true,
                    TotalTargetCount       = 3,
                    TotalAllocatedAmount   = 60000,
                    UnallocatedAmount      = 0,
                    IsFullyAllocated       = true,
                    ReversalReason         = "Driver values (revenue figures) were from incorrect period. Re-run with corrected Q1 actuals in ALLOC-2026-005R.",
                    PreparedByUserId       = UserFPA,
                    ApprovedByUserId       = UserCtrl,
                    ApprovedOn             = new DateTime(2026, 3, 5),
                    AttachmentCount        = 1,
                    CreatedAt              = new DateTime(2026, 2, 28, 9, 0, 0),
                    CreatedBy              = UserFPA,
                    Lines = new()
                    {
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 10, TargetCostCenterId = CC_Sales, TargetCostCenterName = "Sales & Marketing",    BasisValue = 5000000, AllocationPercent = 50, AllocatedAmount = 30000, AllocationLineStatus = AllocationLineStatus.Reversed },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 20, TargetCostCenterId = CC_Admin, TargetCostCenterName = "Administration",       BasisValue = 2000000, AllocationPercent = 20, AllocatedAmount = 12000, AllocationLineStatus = AllocationLineStatus.Reversed },
                        new() { CostAllocationLineId = Guid.NewGuid(), LineNumber = 30, TargetCostCenterId = CC_Prod,  TargetCostCenterName = "Production / Factory", BasisValue = 3000000, AllocationPercent = 30, AllocatedAmount = 18000, AllocationLineStatus = AllocationLineStatus.Reversed },
                    }
                },
            };
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        public static CostAllocationListDto ToListDto(CostAllocationViewModel a) => new()
        {
            CostAllocationId = a.CostAllocationId,
            AllocationCode = a.AllocationCode,
            AllocationName = a.AllocationName,
            AllocationType = a.AllocationType,
            AllocationStatus = a.AllocationStatus,
            AllocationMethod = a.AllocationMethod,
            AllocationDate = a.AllocationDate,
            EffectiveDate = a.EffectiveDate,
            SourceAmount = a.SourceAmount,
            TotalAllocatedAmount = a.TotalAllocatedAmount,
            IsFullyAllocated = a.IsFullyAllocated,
            IsLocked = a.IsLocked,
            TotalTargetCount = a.TotalTargetCount,
            SourceCostCenterName = a.SourceCostCenterId.HasValue
                                        ? CostCenterNames.GetValueOrDefault(a.SourceCostCenterId.Value)
                                        : null,
            CreatedAt = a.CreatedAt,
            AllocationAssumptionText = a.AllocationAssumptionText,
            Notes = a.Notes,
            ReversalReason = a.ReversalReason,
            ApprovedOn = a.ApprovedOn,
            SubmittedOn = a.SubmittedOn,
            LockedOn = a.LockedOn,
            PreviousAllocationId = a.PreviousAllocationId,
            SourceReferenceText = a.SourceReferenceText,
            ScopeType = a.ScopeType,
            SourceAmountType = a.SourceAmountType,
            RoundingRule = a.RoundingRule,
            MustFullyAllocateSource = a.MustFullyAllocateSource,
            UnallocatedAmount = a.UnallocatedAmount,
            AttachmentCount = a.AttachmentCount,
            Lines = a.Lines,
        };
    }
}
