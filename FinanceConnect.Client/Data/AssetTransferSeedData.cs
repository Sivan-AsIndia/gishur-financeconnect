using static FinanceConnect.Client.ViewModels.AssetTransformViewModel;

namespace FinanceConnect.Client.Data
{
    public static class AssetTransferSeedData
    {
        private static readonly Guid Co1  = Guid.Parse("cccccccc-0000-0000-0000-000000000001");

        private static readonly Guid Br1  = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid Br2  = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid Br3  = Guid.Parse("33333333-3333-3333-3333-333333333333");

        private static readonly Guid Fa1  = Guid.Parse("50000000-0000-0000-0000-000000000001");
        private static readonly Guid Fa2  = Guid.Parse("50000000-0000-0000-0000-000000000002");
        private static readonly Guid Fa3  = Guid.Parse("50000000-0000-0000-0000-000000000003");
        private static readonly Guid Fa4  = Guid.Parse("50000000-0000-0000-0000-000000000004");
        private static readonly Guid Fa6  = Guid.Parse("50000000-0000-0000-0000-000000000006");

        public static List<AssetTransferListDto> GetAllAssetTransfers()
        {
            return new List<AssetTransferListDto>
            {
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000001-0000-0000-0000-000000000001"),
                    TransferNumber        = "FATR-000001",
                    TransferStatus        = TransferStatus.Posted,
                    TransferType          = TransferType.CustodianChange,
                    EffectiveTransferDate = new DateTime(2024, 2, 1),
                    FixedAssetId          = Fa1,
                    AssetNumberSnapshot   = "FA-LAP-001",
                    AssetNameSnapshot     = "Dell Latitude Laptop",
                    FromBranchName        = "Head Office",
                    FromLocationName      = "Head Office - IT Room",
                    FromCustodianName     = "IT Department",
                    ToBranchName          = "Head Office",
                    ToLocationName        = "Head Office - IT Room",
                    ToCustodianName       = "Arjun Dev",
                    TransferReason        = "New developer onboarding",
                    AppliedToAssetFlag    = true,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 1, 28),
                },
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000002-0000-0000-0000-000000000002"),
                    TransferNumber        = "FATR-000002",
                    TransferStatus        = TransferStatus.Approved,
                    TransferType          = TransferType.BranchChange,
                    EffectiveTransferDate = new DateTime(2024, 3, 15),
                    FixedAssetId          = Fa2,
                    AssetNumberSnapshot   = "FA-DESK-001",
                    AssetNameSnapshot     = "HP Desktop System",
                    FromBranchName        = "Branch Office",
                    FromLocationName      = "Branch Office - Admin",
                    FromCustodianName     = "Admin Department",
                    ToBranchName          = "Head Office",
                    ToLocationName        = "Head Office - Finance",
                    ToCustodianName       = "Finance Team",
                    TransferReason        = "Branch restructuring",
                    AppliedToAssetFlag    = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 3, 10),
                },
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000003-0000-0000-0000-000000000003"),
                    TransferNumber        = "FATR-000003",
                    TransferStatus        = TransferStatus.Draft,
                    TransferType          = TransferType.LocationChange,
                    EffectiveTransferDate = new DateTime(2024, 4, 1),
                    FixedAssetId          = Fa3,
                    AssetNumberSnapshot   = "FA-FURN-001",
                    AssetNameSnapshot     = "Office Workstation Table",
                    FromBranchName        = "Head Office",
                    FromLocationName      = "Head Office - Work Area",
                    FromCustodianName     = "Operations",
                    ToBranchName          = "Head Office",
                    ToLocationName        = "Head Office - Conference Room",
                    ToCustodianName       = "Operations",
                    TransferReason        = "Office space reorganization",
                    AppliedToAssetFlag    = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 3, 25),
                },
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000004-0000-0000-0000-000000000004"),
                    TransferNumber        = "FATR-000004",
                    TransferStatus        = TransferStatus.Received,
                    TransferType          = TransferType.FullReassignment,
                    EffectiveTransferDate = new DateTime(2024, 3, 20),
                    FixedAssetId          = Fa4,
                    AssetNumberSnapshot   = "FA-VEH-001",
                    AssetNameSnapshot     = "Toyota Innova Company Car",
                    FromBranchName        = "Transport Department",
                    FromLocationName      = "Transport Department",
                    FromCustodianName     = "Logistics Manager",
                    ToBranchName          = "Coimbatore Branch",
                    ToLocationName        = "Coimbatore - Parking",
                    ToCustodianName       = "Coimbatore Manager",
                    TransferReason        = "Regional manager assignment",
                    AppliedToAssetFlag    = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 3, 15),
                },
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000005-0000-0000-0000-000000000005"),
                    TransferNumber        = "FATR-000005",
                    TransferStatus        = TransferStatus.Submitted,
                    TransferType          = TransferType.CustodianChange,
                    EffectiveTransferDate = new DateTime(2024, 4, 5),
                    FixedAssetId          = Fa6,
                    AssetNumberSnapshot   = "FA-LAP-002",
                    AssetNameSnapshot     = "Lenovo ThinkPad Laptop",
                    FromBranchName        = "Branch Office",
                    FromLocationName      = "Branch Office - IT",
                    FromCustodianName     = "IT Support",
                    ToBranchName          = "Branch Office",
                    ToLocationName        = "Branch Office - IT",
                    ToCustodianName       = "Priya S",
                    TransferReason        = "Staff reassignment",
                    AppliedToAssetFlag    = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 3, 30),
                },
                new AssetTransferListDto
                {
                    AssetTransferId       = Guid.Parse("aa000006-0000-0000-0000-000000000006"),
                    TransferNumber        = "FATR-000006",
                    TransferStatus        = TransferStatus.Rejected,
                    TransferType          = TransferType.BranchChange,
                    EffectiveTransferDate = new DateTime(2024, 2, 20),
                    FixedAssetId          = Fa1,
                    AssetNumberSnapshot   = "FA-LAP-001",
                    AssetNameSnapshot     = "Dell Latitude Laptop",
                    FromBranchName        = "Head Office",
                    FromLocationName      = "Head Office - IT Room",
                    FromCustodianName     = "Arjun Dev",
                    ToBranchName          = "Chennai Branch",
                    ToLocationName        = "Chennai - Development",
                    ToCustodianName       = "Arjun Dev",
                    TransferReason        = "Project deployment",
                    AppliedToAssetFlag    = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2024, 2, 18),
                },
            };
        }
    }
}
