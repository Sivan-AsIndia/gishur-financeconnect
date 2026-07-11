using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceConnect.Client.Data
{
    public class CashTransferServiceData
    {
        // In-memory dummy data
        public static List<CashTransferModel> Get()
        {
            var TranferData = new List<CashTransferModel>();
            return new List<CashTransferModel>
            {
          new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00001",
        TransferDate = DateTime.Today.AddDays(-1),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 50000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3000,
        ReceivedOn = new DateTime(2026, 4, 12),
        ReceivedByUserName = "Dani",
        HandedOverByUserName = "Rani",
        HandedOverOn = new DateTime(2026, 1, 11)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00002",
        TransferDate = DateTime.Today.AddDays(-2),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 52000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3200,
        ReceivedOn = new DateTime(2026, 4, 13),
        ReceivedByUserName = "Ravi",
        HandedOverByUserName = "Sita",
        HandedOverOn = new DateTime(2026, 1, 12)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00003",
        TransferDate = DateTime.Today.AddDays(-3),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 53000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3300,
        ReceivedOn = new DateTime(2026, 4, 14),
        ReceivedByUserName = "Kiran",
        HandedOverByUserName = "Meera",
        HandedOverOn = new DateTime(2026, 1, 13)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00004",
        TransferDate = DateTime.Today.AddDays(-4),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 54000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3400,
        ReceivedOn = new DateTime(2026, 4, 15),
        ReceivedByUserName = "Arun",
        HandedOverByUserName = "Priya",
        HandedOverOn = new DateTime(2026, 1, 14)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00005",
        TransferDate = DateTime.Today.AddDays(-5),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 55000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3500,
        ReceivedOn = new DateTime(2026, 4, 16),
        ReceivedByUserName = "Mani",
        HandedOverByUserName = "Vani",
        HandedOverOn = new DateTime(2026, 1, 15)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00006",
        TransferDate = DateTime.Today.AddDays(-6),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 56000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3600,
        ReceivedOn = new DateTime(2026, 4, 17),
        ReceivedByUserName = "Deepa",
        HandedOverByUserName = "Anil",
        HandedOverOn = new DateTime(2026, 1, 16)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00007",
        TransferDate = DateTime.Today.AddDays(-7),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 57000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3700,
        ReceivedOn = new DateTime(2026, 4, 18),
        ReceivedByUserName = "Naveen",
        HandedOverByUserName = "Rekha",
        HandedOverOn = new DateTime(2026, 1, 17)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00008",
        TransferDate = DateTime.Today.AddDays(-8),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 58000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3800,
        ReceivedOn = new DateTime(2026, 4, 19),
        ReceivedByUserName = "Ramesh",
        HandedOverByUserName = "Latha",
        HandedOverOn = new DateTime(2026, 1, 18)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00009",
        TransferDate = DateTime.Today.AddDays(-9),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 59000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 3900,
        ReceivedOn = new DateTime(2026, 4, 20),
        ReceivedByUserName = "Sundar",
        HandedOverByUserName = "Divya",
        HandedOverOn = new DateTime(2026, 1, 19)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00010",
        TransferDate = DateTime.Today.AddDays(-10),
        SourceCashAccountName = "HO Cash",
        DestinationCashAccountName = "Site Cash",
        Amount = 60000,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Draft,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 4000,
        ReceivedOn = new DateTime(2026, 4, 21),
        ReceivedByUserName = "Karthik",
        HandedOverByUserName = "Ananya",
        HandedOverOn = new DateTime(2026, 1, 20)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00011",
        TransferDate = DateTime.Today.AddDays(-11),
        SourceCashAccountName = "Operations Cash",
        DestinationCashAccountName = "Petty Cash",
        Amount = 25000,
        BranchName = "Bengaluru",
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Approved,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Approved,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Security Van",
        ReceivedAmount = 25000,
        ReceivedOn = new DateTime(2026, 4, 22),
        ReceivedByUserName = "Suresh",
        HandedOverByUserName = "Lakshmi",
        HandedOverOn = new DateTime(2026, 1, 21)
    },
    new CashTransferModel
    {
        CashTransferId = Guid.NewGuid(),
        CashTransferNumber = "CASHTRF-00012",
        TransferDate = DateTime.Today.AddDays(-12),
        SourceCashAccountName = "Branch Cash Reserve",
        DestinationCashAccountName = "HO Cash",
        Amount = 45000,
        BranchName = "Hyderabad",
        CashTransferStatus = CashTransferModel.CashTransferStatusEnum.Posted,
        PostingStatus = CashTransferModel.CashTransferStatusEnum.Posted,
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        TransitMethod = "Hand Carry",
        ReceivedAmount = 45000,
        ReceivedOn = new DateTime(2026, 4, 23),
        ReceivedByUserName = "Prasad",
        HandedOverByUserName = "Geetha",
        HandedOverOn = new DateTime(2026, 1, 22)
    }
            };
        }


    }
}

