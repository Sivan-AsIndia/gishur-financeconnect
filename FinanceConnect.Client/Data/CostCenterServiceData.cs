using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class CostCenterServiceData
    {
        public static List<CostCenterModel> Get()
        {
            var hqBranchId = MasterDataIds.Branches.SofaCraftHQ;
            var blrBranchId = MasterDataIds.Branches.SofaCraftBengaluru;
            var hydBranchId = MasterDataIds.Branches.CozyCraftHyderabad;
            var mumBranchId = MasterDataIds.Branches.UrbanLoftMumbai;
            var delBranchId = MasterDataIds.Branches.PlushComfortDelhi;

            var adminHOId = Guid.NewGuid();
            var salesChnId = Guid.NewGuid();
            var hrId = Guid.NewGuid();
            var itId = Guid.NewGuid();
            var salesBlrId = Guid.NewGuid();
            var opsBlrId = Guid.NewGuid();
            var financeHOId = Guid.NewGuid();
            var mktHOId = Guid.NewGuid();
            var salesHydId = Guid.NewGuid();
            var salesMumId = Guid.NewGuid();
            var salesDelId = Guid.NewGuid();
            var sharedSvcId = Guid.NewGuid();

            return new List<CostCenterModel>
            {
                // 1. Head Office Admin (Root)
                new CostCenterModel
                {
                    Id = adminHOId,
                    CostCenterCode = "CC-ADMIN-HO",
                    CostCenterName = "Head Office Administration",
                    ShortName = "HO-Admin",
                    Description = "Central administration cost center for head office overhead",
                    CostCenterType = "Administrative",
                    ControlNature = "CostOnly",
                    UsageMode = "BudgetAndActual",
                    IsSharedServiceCenter = false,
                    IsAllocationTargetAllowed = true,
                    IsAllocationSourceAllowed = false,
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-ADMIN-HO",
                    CostCenterOwnerName = "Priya Sharma",
                    ReportingGroupCode = "ADMIN",
                    AllocationBaseType = "None",
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 5,
                    IsCapexAllowed = true,
                    IsOpexAllowed = true,
                    AllowNegativeBalance = false,
                    BankGLAccountCode = null,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    IsLocked = false,
                    CreatedAt = DateTime.Now.AddDays(-120)
                },

                // 2. Human Resources
                new CostCenterModel
                {
                    Id = hrId,
                    CostCenterCode = "CC-HR-001",
                    CostCenterName = "Human Resources",
                    ShortName = "HR",
                    Description = "HR department — salary, recruitment, training",
                    CostCenterType = "Administrative",
                    ControlNature = "CostOnly",
                    UsageMode = "BudgetAndActual",
                    ParentCostCenterId = adminHOId,
                    ParentCostCenterName = "Head Office Administration",
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 2,
                    HierarchyPath = "CC-ADMIN-HO/CC-HR-001",
                    CostCenterOwnerName = "Ramesh Kumar",
                    ReportingGroupCode = "ADMIN",
                    AllocationBaseType = "Headcount",
                    DefaultAllocationDriverValue = 120,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 5,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-115)
                },

                // 3. IT Department
                new CostCenterModel
                {
                    Id = itId,
                    CostCenterCode = "CC-IT-001",
                    CostCenterName = "Information Technology",
                    ShortName = "IT",
                    Description = "IT infrastructure, software licenses, support",
                    CostCenterType = "Administrative",
                    ControlNature = "CostOnly",
                    UsageMode = "BudgetAndActual",
                    ParentCostCenterId = adminHOId,
                    ParentCostCenterName = "Head Office Administration",
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 2,
                    HierarchyPath = "CC-ADMIN-HO/CC-IT-001",
                    CostCenterOwnerName = "Arun Venkat",
                    ReportingGroupCode = "ADMIN",
                    AllocationBaseType = "Usage",
                    DefaultAllocationDriverValue = 80,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "HardControl",
                    TolerancePercent = 2,
                    ToleranceAmount = 50000,
                    IsCapexAllowed = true,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-110)
                },

                // 4. Shared Service Center
                new CostCenterModel
                {
                    Id = sharedSvcId,
                    CostCenterCode = "CC-SHARED-HO",
                    CostCenterName = "Shared Services",
                    ShortName = "Shared-Svc",
                    Description = "Common facility costs — rent, utilities, housekeeping",
                    CostCenterType = "SharedService",
                    ControlNature = "CostOnly",
                    UsageMode = "AllocationOnly",
                    IsSharedServiceCenter = true,
                    IsAllocationSourceAllowed = true,
                    IsAllocationTargetAllowed = false,
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SHARED-HO",
                    CostCenterOwnerName = "Priya Sharma",
                    ReportingGroupCode = "SUPPORT",
                    AllocationBaseType = "FloorArea",
                    DefaultAllocationDriverValue = 5000,
                    CanReceiveSharedCost = false,
                    CanDistributeSharedCost = true,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "Advisory",
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-118)
                },

                // 5. Finance HO
                new CostCenterModel
                {
                    Id = financeHOId,
                    CostCenterCode = "CC-FIN-HO",
                    CostCenterName = "Finance & Accounts",
                    ShortName = "Finance",
                    Description = "Finance, accounting, compliance",
                    CostCenterType = "Administrative",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    ParentCostCenterId = adminHOId,
                    ParentCostCenterName = "Head Office Administration",
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 2,
                    HierarchyPath = "CC-ADMIN-HO/CC-FIN-HO",
                    CostCenterOwnerName = "Kavitha Nair",
                    ApprovalRoleCode = "CFOReview",
                    ReportingGroupCode = "ADMIN",
                    AllocationBaseType = "None",
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "HardControl",
                    TolerancePercent = 1,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-112)
                },

                // 6. Chennai Sales
                new CostCenterModel
                {
                    Id = salesChnId,
                    CostCenterCode = "CC-SALES-CHN",
                    CostCenterName = "Chennai Sales Division",
                    ShortName = "Sales-CHN",
                    Description = "Sales operations for Chennai region",
                    CostCenterType = "Operational",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SALES-CHN",
                    CostCenterOwnerName = "Suresh Babu",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "RevenueShare",
                    DefaultAllocationDriverValue = 35,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 8,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-100)
                },

                // 7. Bangalore Sales
                new CostCenterModel
                {
                    Id = salesBlrId,
                    CostCenterCode = "CC-SALES-BLR",
                    CostCenterName = "Bengaluru Sales Division",
                    ShortName = "Sales-BLR",
                    Description = "Sales operations for Bengaluru region",
                    CostCenterType = "Operational",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    BranchId = blrBranchId,
                    BranchName = SeedLookup.BranchName(blrBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SALES-BLR",
                    CostCenterOwnerName = "Deepak Rao",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "RevenueShare",
                    DefaultAllocationDriverValue = 28,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 8,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-95)
                },

                // 8. Bangalore Operations
                new CostCenterModel
                {
                    Id = opsBlrId,
                    CostCenterCode = "CC-OPS-BLR",
                    CostCenterName = "Bengaluru Operations",
                    ShortName = "Ops-BLR",
                    Description = "Delivery and operations for Bengaluru",
                    CostCenterType = "Operational",
                    ControlNature = "CostOnly",
                    UsageMode = "BudgetAndActual",
                    BranchId = blrBranchId,
                    BranchName = SeedLookup.BranchName(blrBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-OPS-BLR",
                    CostCenterOwnerName = "Meera Pillai",
                    ReportingGroupCode = "DELIVERY",
                    AllocationBaseType = "Headcount",
                    DefaultAllocationDriverValue = 45,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 5,
                    IsCapexAllowed = true,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-90)
                },

                // 9. Marketing HO
                new CostCenterModel
                {
                    Id = mktHOId,
                    CostCenterCode = "CC-MKT-HO",
                    CostCenterName = "Marketing",
                    ShortName = "Marketing",
                    Description = "Brand, digital marketing, campaigns",
                    CostCenterType = "Administrative",
                    ControlNature = "CostOnly",
                    UsageMode = "BudgetAndActual",
                    BranchId = hqBranchId,
                    BranchName = SeedLookup.BranchName(hqBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-MKT-HO",
                    CostCenterOwnerName = "Ananya Seth",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "None",
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "HardControl",
                    TolerancePercent = 3,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-85)
                },

                // 10. Hyderabad Sales
                new CostCenterModel
                {
                    Id = salesHydId,
                    CostCenterCode = "CC-SALES-HYD",
                    CostCenterName = "Hyderabad Sales Division",
                    ShortName = "Sales-HYD",
                    Description = "Sales for Hyderabad region",
                    CostCenterType = "Operational",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    BranchId = hydBranchId,
                    BranchName = SeedLookup.BranchName(hydBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SALES-HYD",
                    CostCenterOwnerName = "Ravi Teja",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "RevenueShare",
                    DefaultAllocationDriverValue = 18,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 10,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Active",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-80)
                },

                // 11. Mumbai Sales — Inactive
                new CostCenterModel
                {
                    Id = salesMumId,
                    CostCenterCode = "CC-SALES-MUM",
                    CostCenterName = "Mumbai Sales Division",
                    ShortName = "Sales-MUM",
                    Description = "Sales operations Mumbai — currently inactive",
                    CostCenterType = "Operational",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    BranchId = mumBranchId,
                    BranchName = SeedLookup.BranchName(mumBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SALES-MUM",
                    CostCenterOwnerName = "Nandini Joshi",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "None",
                    CanReceiveSharedCost = false,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "ReportingOnly",
                    IsCapexAllowed = false,
                    IsOpexAllowed = false,
                    EffectiveFrom = new DateTime(2023, 1, 1),
                    EffectiveTo = new DateTime(2024, 12, 31),
                    CostCenterStatus = "Inactive",
                    IsActive = false,
                    ClosureReason = "Region merged into Chennai division",
                    ReplacedByCostCenterId = salesChnId,
                    ReplacedByCostCenterName = "Chennai Sales Division",
                    CreatedAt = DateTime.Now.AddDays(-200)
                },

                // 12. Delhi Sales — Locked
                new CostCenterModel
                {
                    Id = salesDelId,
                    CostCenterCode = "CC-SALES-DEL",
                    CostCenterName = "Delhi Sales Division",
                    ShortName = "Sales-DEL",
                    Description = "Sales operations for Delhi NCR",
                    CostCenterType = "Operational",
                    ControlNature = "Mixed",
                    UsageMode = "BudgetAndActual",
                    BranchId = delBranchId,
                    BranchName = SeedLookup.BranchName(delBranchId),
                    HierarchyLevel = 1,
                    HierarchyPath = "CC-SALES-DEL",
                    CostCenterOwnerName = "Vikram Singh",
                    ApprovalRoleCode = "RegionalController",
                    ReportingGroupCode = "SALES",
                    AllocationBaseType = "RevenueShare",
                    DefaultAllocationDriverValue = 19,
                    CanReceiveSharedCost = true,
                    CanDistributeSharedCost = false,
                    DefaultCurrencyCode = "INR",
                    BudgetControlMode = "SoftControl",
                    TolerancePercent = 8,
                    IsCapexAllowed = false,
                    IsOpexAllowed = true,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    CostCenterStatus = "Locked",
                    IsActive = true,
                    IsLocked = true,
                    LockedOn = DateTime.Now.AddDays(-10),
                    LockedByName = "Kavitha Nair",
                    Notes = "Locked pending audit review Q3",
                    CreatedAt = DateTime.Now.AddDays(-75)
                }
            };
        }
    }
}
