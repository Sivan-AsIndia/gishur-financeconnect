using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class AssetAcquisitionSeedData
    {
        public static List<AssetAcquisitionViewModel.AssetAcquisition> GetAll()
        {
            return new List<AssetAcquisitionViewModel.AssetAcquisition>
            {
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000001",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2023, 6, 12),
                    CapitalizationDate = new DateTime(2023, 6, 15),
                    PostingDate = new DateTime(2023, 6, 15),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    AssetNumberSnapshot = "FA-LAP-001",
                    AssetNameSnapshot = "Dell Latitude Laptop",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.AP,
                    VendorId = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    VendorInvoiceNumber = "INV-2023-5567",
                    VendorInvoiceDate = new DateTime(2023, 6, 10),
                    PurchaseOrderRef = "PO-2023-1201",
                    ReferenceText = "IT procurement – Q2 budget",
                    Narration = "Initial capitalization of Dell Latitude laptop for development team",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    JournalEntryId = Guid.Parse("e0000000-0000-0000-0000-000000000001"),
                    PostedOn = new DateTime(2023, 6, 15),
                    RoundOffAmount = 0,
                    AttachmentCount = 2,
                    CreatedAt = new DateTime(2023, 6, 12),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Dell Latitude 5540 base unit", LineAmount = 60000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000002"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Freight, LineDescription = "Shipping charges", LineAmount = 1500m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000003"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), LineNumber = 30, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Installation, LineDescription = "OS Setup & configuration", LineAmount = 3500m, IsCapitalizable = true }
                    }
                },

                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000002"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    AcquisitionNumber = "FAACQ-000002",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Approved,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2023, 8, 20),
                    CapitalizationDate = new DateTime(2023, 8, 25),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    AssetNumberSnapshot = "FA-DESK-001",
                    AssetNameSnapshot = "HP Desktop System",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    VendorInvoiceNumber = "INV-2023-7890",
                    VendorInvoiceDate = new DateTime(2023, 8, 18),
                    Narration = "Desktop acquisition for branch admin",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                    RoundOffAmount = 0,
                    AttachmentCount = 1,
                    CreatedAt = new DateTime(2023, 8, 20),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000004"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000002"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "HP ProDesk 400 G9 base unit", LineAmount = 45000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000005"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000002"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Installation, LineDescription = "Workstation setup", LineAmount = 3000m, IsCapitalizable = true }
                    }
                },

                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000003"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000003",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Draft,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.CapitalImprovement,
                    AcquisitionDate = new DateTime(2024, 3, 10),
                    CapitalizationDate = new DateTime(2024, 3, 15),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    AssetNumberSnapshot = "FA-LAP-001",
                    AssetNameSnapshot = "Dell Latitude Laptop",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    Narration = "RAM upgrade from 16GB to 32GB",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    RoundOffAmount = 0,
                    AttachmentCount = 0,
                    CreatedAt = new DateTime(2024, 3, 10),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000006"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000003"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "32GB DDR5 RAM Module", LineAmount = 8500m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000007"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000003"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.ProfessionalFee, LineDescription = "Technician service charge", LineAmount = 500m, IsCapitalizable = true }
                    }
                },

                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000004"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000004",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.OpeningBalance,
                    AcquisitionDate = new DateTime(2023, 4, 1),
                    CapitalizationDate = new DateTime(2023, 4, 1),
                    PostingDate = new DateTime(2023, 4, 1),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    AssetNumberSnapshot = "FA-FURN-001",
                    AssetNameSnapshot = "Office Workstation Table",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.System,
                    Narration = "Opening balance entry during go-live migration",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000007"),
                    JournalEntryId = Guid.Parse("e0000000-0000-0000-0000-000000000002"),
                    PostedOn = new DateTime(2023, 4, 1),
                    RoundOffAmount = 0,
                    AttachmentCount = 1,
                    CreatedAt = new DateTime(2023, 4, 1),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000008"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000004"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Opening balance – furniture cost", LineAmount = 25000m, IsCapitalizable = true }
                    }
                },

                // ── 5: Submitted – Lease Acquisition ─────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000005"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    AcquisitionNumber = "FAACQ-000005",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Submitted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.TransferIn,
                    AcquisitionDate = new DateTime(2024, 1, 15),
                    CapitalizationDate = new DateTime(2024, 1, 20),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    AssetNumberSnapshot = "FA-VEH-001",
                    AssetNameSnapshot = "Toyota Innova Fleet Vehicle",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    VendorInvoiceNumber = "LEASE-2024-0012",
                    VendorInvoiceDate = new DateTime(2024, 1, 10),
                    PurchaseOrderRef = "PO-2024-0055",
                    Narration = "Lease capitalization of fleet vehicle for field operations",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000010"),
                    RoundOffAmount = 0,
                    AttachmentCount = 3,
                    CreatedAt = new DateTime(2024, 1, 15),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000009"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000005"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Vehicle base price – Innova Crysta", LineAmount = 1850000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000010"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000005"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.NonCapitalizable, LineDescription = "Comprehensive motor insurance – Year 1", LineAmount = 42000m, IsCapitalizable = false }
                    }
                },

                // ── 6: Posted – Transfer from CWIP ────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000006"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000006",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.SelfConstructed,
                    AcquisitionDate = new DateTime(2024, 5, 1),
                    CapitalizationDate = new DateTime(2024, 5, 5),
                    PostingDate = new DateTime(2024, 5, 5),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    AssetNumberSnapshot = "FA-SERV-001",
                    AssetNameSnapshot = "Dell PowerEdge R750 Server",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.AP,
                    VendorId = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    VendorInvoiceNumber = "INV-2024-3120",
                    VendorInvoiceDate = new DateTime(2024, 4, 28),
                    PurchaseOrderRef = "PO-2024-0102",
                    ReferenceText = "Data center expansion – Phase 1",
                    Narration = "Server transfer from CWIP to fixed asset after commissioning",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.CWIPToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000012"),
                    JournalEntryId = Guid.Parse("e0000000-0000-0000-0000-000000000003"),
                    PostedOn = new DateTime(2024, 5, 5),
                    RoundOffAmount = 0,
                    AttachmentCount = 4,
                    CreatedAt = new DateTime(2024, 5, 1),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000011"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000006"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Dell PowerEdge R750 hardware", LineAmount = 420000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000012"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000006"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Installation, LineDescription = "Rack mounting & cabling", LineAmount = 15000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000013"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000006"), LineNumber = 30, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.ProfessionalFee, LineDescription = "OS licensing & configuration", LineAmount = 35000m, IsCapitalizable = true }
                    }
                },

                // ── 7: Rejected – Acquisition ─────────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000007"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    AcquisitionNumber = "FAACQ-000007",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Rejected,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2024, 7, 8),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    AssetNumberSnapshot = "FA-PROJ-001",
                    AssetNameSnapshot = "Epson EB-X51 Projector",
                    AssetStatusSnapshot = "Draft",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    Narration = "Projector for conference room – rejected due to budget constraints",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000014"),
                    RoundOffAmount = 0,
                    AttachmentCount = 0,
                    CreatedAt = new DateTime(2024, 7, 8),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000014"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000007"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Epson EB-X51 projector unit", LineAmount = 72000m, IsCapitalizable = true }
                    }
                },

                // ── 8: Posted – Donation Received ─────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000008"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000008",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.Donation,
                    AcquisitionDate = new DateTime(2024, 9, 1),
                    CapitalizationDate = new DateTime(2024, 9, 5),
                    PostingDate = new DateTime(2024, 9, 5),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000007"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    AssetNumberSnapshot = "FA-LAP-002",
                    AssetNameSnapshot = "MacBook Pro 14-inch",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    ReferenceText = "Donation from partner company",
                    Narration = "MacBook received as donation for design team",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    JournalEntryId = Guid.Parse("e0000000-0000-0000-0000-000000000004"),
                    PostedOn = new DateTime(2024, 9, 5),
                    RoundOffAmount = 0,
                    AttachmentCount = 1,
                    CreatedAt = new DateTime(2024, 9, 1),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000015"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000008"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "MacBook Pro – fair market value", LineAmount = 185000m, IsCapitalizable = true }
                    }
                },

                // ── 9: Approved – Revaluation Capitalization ──────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000009"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000009",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Approved,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.CapitalImprovement,
                    AcquisitionDate = new DateTime(2024, 11, 5),
                    CapitalizationDate = new DateTime(2024, 11, 10),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    AssetNumberSnapshot = "FA-SERV-001",
                    AssetNameSnapshot = "Dell PowerEdge R750 Server",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.AP,
                    VendorInvoiceNumber = "INV-2024-9001",
                    VendorInvoiceDate = new DateTime(2024, 11, 2),
                    Narration = "SSD upgrade – 2TB NVMe storage expansion for server",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000012"),
                    RoundOffAmount = 0,
                    AttachmentCount = 1,
                    CreatedAt = new DateTime(2024, 11, 5),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000016"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000009"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "2TB NVMe SSD module", LineAmount = 28000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000017"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000009"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Installation, LineDescription = "Installation & data migration", LineAmount = 5000m, IsCapitalizable = true }
                    }
                },

                // ── 10: Draft – New AC Unit ───────────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000010"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    AcquisitionNumber = "FAACQ-000010",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Draft,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2025, 1, 20),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000008"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    AssetNumberSnapshot = "FA-HVAC-001",
                    AssetNameSnapshot = "Daikin Split AC 2 Ton",
                    AssetStatusSnapshot = "Draft",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.Manual,
                    VendorInvoiceNumber = "INV-2025-0205",
                    VendorInvoiceDate = new DateTime(2025, 1, 18),
                    Narration = "New AC unit for server room – pending approval",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000015"),
                    RoundOffAmount = 0,
                    AttachmentCount = 0,
                    CreatedAt = new DateTime(2025, 1, 20),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000018"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000010"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "Daikin 2-Ton inverter split AC", LineAmount = 62000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000019"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000010"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Installation, LineDescription = "Installation & copper piping", LineAmount = 8500m, IsCapitalizable = true }
                    }
                },

                // ── 11: Cancelled – Printer ───────────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000011"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    AcquisitionNumber = "FAACQ-000011",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Cancelled,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2024, 6, 15),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000009"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                    AssetNumberSnapshot = "FA-PRNT-001",
                    AssetNameSnapshot = "HP LaserJet Pro MFP",
                    AssetStatusSnapshot = "Cancelled",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.AP,
                    VendorInvoiceNumber = "INV-2024-5500",
                    Narration = "Order cancelled – vendor could not deliver within agreed timeline",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000016"),
                    RoundOffAmount = 0,
                    AttachmentCount = 0,
                    CreatedAt = new DateTime(2024, 6, 15),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000020"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000011"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "HP LaserJet Pro MFP M428fdn", LineAmount = 38000m, IsCapitalizable = true }
                    }
                },

                // ── 12: Posted – Office Chair Bulk ────────────────────
                new AssetAcquisitionViewModel.AssetAcquisition
                {
                    AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000012"),
                    TenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BranchId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    AcquisitionNumber = "FAACQ-000012",
                    AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted,
                    AcquisitionType = AssetAcquisitionViewModel.AcquisitionTypeEnum.InitialCapitalization,
                    AcquisitionDate = new DateTime(2024, 2, 10),
                    CapitalizationDate = new DateTime(2024, 2, 15),
                    PostingDate = new DateTime(2024, 2, 15),
                    FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000010"),
                    AssetCategoryIdSnapshot = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    AssetNumberSnapshot = "FA-FURN-002",
                    AssetNameSnapshot = "Ergonomic Office Chair Set",
                    AssetStatusSnapshot = "Active",
                    SourceModule = AssetAcquisitionViewModel.SourceModuleEnum.AP,
                    VendorId = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    VendorInvoiceNumber = "INV-2024-1100",
                    VendorInvoiceDate = new DateTime(2024, 2, 8),
                    PurchaseOrderRef = "PO-2024-0030",
                    ReferenceText = "Office renovation – Phase 2",
                    Narration = "Bulk purchase of 10 ergonomic chairs for new wing",
                    PostingRoute = AssetAcquisitionViewModel.PostingRouteEnum.DirectToAsset,
                    AssetCostGLAccountIdSnapshot = Guid.Parse("30000000-0000-0000-0000-000000000007"),
                    JournalEntryId = Guid.Parse("e0000000-0000-0000-0000-000000000005"),
                    PostedOn = new DateTime(2024, 2, 15),
                    RoundOffAmount = 0,
                    AttachmentCount = 2,
                    CreatedAt = new DateTime(2024, 2, 10),
                    CostLines = new List<AssetAcquisitionViewModel.AssetAcquisitionLine>
                    {
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000021"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000012"), LineNumber = 10, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.BaseCost, LineDescription = "10x Herman Miller Aeron chairs", LineAmount = 150000m, IsCapitalizable = true },
                        new() { AssetAcquisitionLineId = Guid.Parse("b0000000-0000-0000-0000-000000000022"), AssetAcquisitionId = Guid.Parse("a0000000-0000-0000-0000-000000000012"), LineNumber = 20, CostComponentType = AssetAcquisitionViewModel.CostComponentTypeEnum.Freight, LineDescription = "Delivery & assembly charges", LineAmount = 5000m, IsCapitalizable = true }
                    }
                }
            };
        }
    }
}
