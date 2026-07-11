using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;

namespace FinanceConnect.Client.Data
{
    public static class FundTransferServiceData
    {
        public static List<FundTransferModel> Get()
        {
            return new List<FundTransferModel>
            {
                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                } ,new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                }, new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                },
                 new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                },
                 new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                },
                 new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00001",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    Status = FundTransferStatus.Draft,

                    SourceBankAccount = "ICICI-CHN-001",
                    DestinationBankAccount = "HDFC-BLR-002",

                    TransferDate = DateTime.Today.AddDays(-1),
                    SourceValueDate = DateTime.Today.AddDays(-1),
                    DestinationValueDate = DateTime.Today,

                    Amount = 1500000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Branch fund top-up",

                    TransferMethod = TransferMethod.NEFT,

                    RequestedBy = "Ramya"
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00002",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    Status = FundTransferStatus.Submitted,

                    SourceBankAccount = "SBI-MUM-001",
                    DestinationBankAccount = "AXIS-DEL-003",

                    TransferDate = DateTime.Today,
                    SourceValueDate = DateTime.Today,
                    DestinationValueDate = DateTime.Today,

                    Amount = 250000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Vendor payment",

                    TransferMethod = TransferMethod.RTGS,

                    RequestedBy = "Arjun",
                    SubmittedOn = DateTime.Now.AddHours(-3)
                },

                new FundTransferModel
                {
                    FundTransferId = Guid.NewGuid(),
                    FundTransferNumber = "FNDTRF-00003",
                    Company = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    Branch = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    Status = FundTransferStatus.Posted,

                    SourceBankAccount = "YES-BLR-005",
                    DestinationBankAccount = "KOTAK-CHN-004",

                    TransferDate = DateTime.Today.AddDays(-3),
                    SourceValueDate = DateTime.Today.AddDays(-3),
                    DestinationValueDate = DateTime.Today.AddDays(-3),

                    Amount = 5000000,
                    Currency = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    Narration = "Payroll funding",

                    TransferMethod = TransferMethod.IMPS,

                    RequestedBy = "Finance",

                    ApprovedBy = "Controller1",
                    ApprovedOn = DateTime.Today.AddDays(-2)
                }
            };
        }
    }
}
