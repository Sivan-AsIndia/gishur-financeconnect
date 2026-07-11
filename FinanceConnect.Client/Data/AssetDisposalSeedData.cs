using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class AssetDisposalSeedData
    {
        public List<AssetDisposalViewModel> Seed()
        {
            return new List<AssetDisposalViewModel>
            {

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                    DisposalNumber = "DSP-000001",
                    AssetNumberSnapshot = "FA-1001",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today.AddDays(-30),
                    TotalCapitalizedCostSnapshot = 100000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 70000,
                    NetBookValueAsOfDisposalSnapshot = 30000,
                    ProceedsAmount = 40000,
                    NetProceedsAmount = 40000,
                    GainLossAmount = 10000,
                    DisposalStatus = AssetDisposalStatus.Posted,
                    CreatedAt = DateTime.UtcNow.AddDays(-32)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                    DisposalNumber = "DSP-000002",
                    AssetNumberSnapshot = "FA-1002",
                    DisposalType = AssetDisposalType.Scrap,
                    DisposalDate = DateTime.Today.AddDays(-25),
                    TotalCapitalizedCostSnapshot = 50000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 35000,
                    NetBookValueAsOfDisposalSnapshot = 15000,
                    ProceedsAmount = 0,
                    NetProceedsAmount = 0,
                    GainLossAmount = -15000,
                    DisposalStatus = AssetDisposalStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-27)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                    DisposalNumber = "DSP-000003",
                    AssetNumberSnapshot = "FA-1003",
                    DisposalType = AssetDisposalType.WriteOff,
                    DisposalDate = DateTime.Today.AddDays(-20),
                    TotalCapitalizedCostSnapshot = 20000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 15000,
                    NetBookValueAsOfDisposalSnapshot = 5000,
                    ProceedsAmount = 0,
                    NetProceedsAmount = 0,
                    GainLossAmount = -5000,
                    DisposalStatus = AssetDisposalStatus.Draft,
                    CreatedAt = DateTime.UtcNow.AddDays(-21)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                    DisposalNumber = "DSP-000004",
                    AssetNumberSnapshot = "FA-1004",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today.AddDays(-18),
                    TotalCapitalizedCostSnapshot = 75000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 50000,
                    NetBookValueAsOfDisposalSnapshot = 25000,
                    ProceedsAmount = 30000,
                    NetProceedsAmount = 30000,
                    GainLossAmount = 5000,
                    DisposalStatus = AssetDisposalStatus.Posted,
                    CreatedAt = DateTime.UtcNow.AddDays(-19)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                    DisposalNumber = "DSP-000005",
                    AssetNumberSnapshot = "FA-1005",
                    DisposalType = AssetDisposalType.TransferOut,
                    DisposalDate = DateTime.Today.AddDays(-15),
                    TotalCapitalizedCostSnapshot = 60000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 45000,
                    NetBookValueAsOfDisposalSnapshot = 15000,
                    ProceedsAmount = 15000,
                    NetProceedsAmount = 15000,
                    GainLossAmount = 0,
                    DisposalStatus = AssetDisposalStatus.Submitted,
                    CreatedAt = DateTime.UtcNow.AddDays(-16)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                    DisposalNumber = "DSP-000006",
                    AssetNumberSnapshot = "FA-1006",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today.AddDays(-12),
                    TotalCapitalizedCostSnapshot = 90000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 65000,
                    NetBookValueAsOfDisposalSnapshot = 25000,
                    ProceedsAmount = 20000,
                    NetProceedsAmount = 20000,
                    GainLossAmount = -5000,
                    DisposalStatus = AssetDisposalStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-13)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000007"),
                    DisposalNumber = "DSP-000007",
                    AssetNumberSnapshot = "FA-1007",
                    DisposalType = AssetDisposalType.WriteOff,
                    DisposalDate = DateTime.Today.AddDays(-10),
                    TotalCapitalizedCostSnapshot = 30000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 28000,
                    NetBookValueAsOfDisposalSnapshot = 2000,
                    ProceedsAmount = 0,
                    NetProceedsAmount = 0,
                    GainLossAmount = -2000,
                    DisposalStatus = AssetDisposalStatus.Posted,
                    CreatedAt = DateTime.UtcNow.AddDays(-11)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000008"),
                    DisposalNumber = "DSP-000008",
                    AssetNumberSnapshot = "FA-1008",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today.AddDays(-8),
                    TotalCapitalizedCostSnapshot = 45000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 30000,
                    NetBookValueAsOfDisposalSnapshot = 15000,
                    ProceedsAmount = 18000,
                    NetProceedsAmount = 18000,
                    GainLossAmount = 3000,
                    DisposalStatus = AssetDisposalStatus.Submitted,
                    CreatedAt = DateTime.UtcNow.AddDays(-9)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000009"),
                    DisposalNumber = "DSP-000009",
                    AssetNumberSnapshot = "FA-1009",
                    DisposalType = AssetDisposalType.Scrap,
                    DisposalDate = DateTime.Today.AddDays(-6),
                    TotalCapitalizedCostSnapshot = 25000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 20000,
                    NetBookValueAsOfDisposalSnapshot = 5000,
                    ProceedsAmount = 500,
                    NetProceedsAmount = 500,
                    GainLossAmount = -4500,
                    DisposalStatus = AssetDisposalStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000010"),
                    DisposalNumber = "DSP-000010",
                    AssetNumberSnapshot = "FA-1010",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today.AddDays(-4),
                    TotalCapitalizedCostSnapshot = 120000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 80000,
                    NetBookValueAsOfDisposalSnapshot = 40000,
                    ProceedsAmount = 45000,
                    NetProceedsAmount = 45000,
                    GainLossAmount = 5000,
                    DisposalStatus = AssetDisposalStatus.Draft,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000011"),
                    DisposalNumber = "DSP-000011",
                    AssetNumberSnapshot = "FA-1011",
                    DisposalType = AssetDisposalType.TransferOut,
                    DisposalDate = DateTime.Today.AddDays(-2),
                    TotalCapitalizedCostSnapshot = 55000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 35000,
                    NetBookValueAsOfDisposalSnapshot = 20000,
                    ProceedsAmount = 20000,
                    NetProceedsAmount = 20000,
                    GainLossAmount = 0,
                    DisposalStatus = AssetDisposalStatus.Draft,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },

                new AssetDisposalViewModel
                {
                    AssetDisposalId = Guid.Parse("50000000-0000-0000-0000-000000000012"),
                    DisposalNumber = "DSP-000012",
                    AssetNumberSnapshot = "FA-1012",
                    DisposalType = AssetDisposalType.Sale,
                    DisposalDate = DateTime.Today,
                    TotalCapitalizedCostSnapshot = 80000,
                    AccumulatedDepreciationAsOfDisposalSnapshot = 60000,
                    NetBookValueAsOfDisposalSnapshot = 20000,
                    ProceedsAmount = 22000,
                    NetProceedsAmount = 22000,
                    GainLossAmount = 2000,
                    DisposalStatus = AssetDisposalStatus.Draft,
                    CreatedAt = DateTime.UtcNow
                }

            };
        }
    }
}