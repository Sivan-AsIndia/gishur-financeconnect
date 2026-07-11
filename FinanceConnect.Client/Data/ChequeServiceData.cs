using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class ChequeServiceData
    {
        public static List<ChequeModel> Get()
        {
            var hqBranch = MasterDataIds.Branches.SofaCraftHQ;
            var blrBranch = MasterDataIds.Branches.SofaCraftBengaluru;

            return new()
            {
                new ChequeModel
                {
                    ChequeNumber = "CHQ001",
                    Amount = 25000,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Issued,
                    CounterpartyName = "ABC Traders",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "HDFC - 8899",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-2),
                    IssuedOn = DateTime.Today.AddDays(-2),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-2),
                    PreparedOn = DateTime.Today.AddDays(-3),
                    PrintedOn = DateTime.Today.AddDays(-3),
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "ABC Traders",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ002",
                    Amount = 18000,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Deposited,
                    CounterpartyName = "Ravi Stores",
                    CounterpartyType = "Customer",
                    OurBankAccount = "ICICI - 4455",
                    BranchId = blrBranch,
                    Branch = "BLR - SofaCraft Experience Store - Bengaluru",
                    ChequeDate = DateTime.Today.AddDays(-3),
                    DepositedOn = DateTime.Today.AddDays(-1),
                    ReceivedOn = DateTime.Today.AddDays(-2),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-2),
                    PreparedOn = DateTime.Today.AddDays(-3),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    DrawerBankName = "SBI",
                    DrawerBankBranch = "Anna Nagar",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ003",
                    Amount = 42000,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Printed,
                    CounterpartyName = "Sri Agencies",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "Axis - 2233",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-5),
                    PreparedOn = DateTime.Today.AddDays(-6),
                    PrintedOn = DateTime.Today.AddDays(-5),
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "Sri Agencies",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ004",
                    Amount = 9600,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Cleared,
                    CounterpartyName = "Kumar Textiles",
                    CounterpartyType = "Customer",
                    OurBankAccount = "SBI - 9911",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-10),
                    DepositedOn = DateTime.Today.AddDays(-4),
                    ClearedOn = DateTime.Today.AddDays(-2),
                    ReceivedOn = DateTime.Today.AddDays(-5),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-5),
                    PreparedOn = DateTime.Today.AddDays(-6),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ005",
                    Amount = 15000,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Bounced,
                    CounterpartyName = "Lotus Suppliers",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "HDFC - 8899",
                    BranchId = blrBranch,
                    Branch = "BLR - SofaCraft Experience Store - Bengaluru",
                    ChequeDate = DateTime.Today.AddDays(-15),
                    IssuedOn = DateTime.Today.AddDays(-10),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-10),
                    PreparedOn = DateTime.Today.AddDays(-12),
                    PrintedOn = DateTime.Today.AddDays(-11),
                    BouncedOn = DateTime.Today.AddDays(-3),
                    BounceReason = "InsufficientFunds",
                    BounceReasonText = "Insufficient funds in account",
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "Lotus Suppliers",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ006",
                    Amount = 30500,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Received,
                    CounterpartyName = "Vijay Enterprises",
                    CounterpartyType = "Customer",
                    OurBankAccount = "ICICI - 4455",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-3),
                    ReceivedOn = DateTime.Today.AddDays(-3),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-3),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    DrawerBankName = "HDFC Bank",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ007",
                    Amount = 22000,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Cancelled,
                    CounterpartyName = "Metro Distributors",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "Axis - 2233",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-20),
                    CancelledOn = DateTime.Today.AddDays(-8),
                    PreparedOn = DateTime.Today.AddDays(-21),
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "Metro Distributors",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ008",
                    Amount = 8750,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Stopped,
                    CounterpartyName = "Ganesh Mart",
                    CounterpartyType = "Customer",
                    OurBankAccount = "SBI - 9911",
                    BranchId = blrBranch,
                    Branch = "BLR - SofaCraft Experience Store - Bengaluru",
                    ChequeDate = DateTime.Today.AddDays(-12),
                    StoppedOn = DateTime.Today.AddDays(-4),
                    ReceivedOn = DateTime.Today.AddDays(-7),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-7),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ009",
                    Amount = 54000,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Draft,
                    CounterpartyName = "Bright Solutions",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "HDFC - 8899",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today,
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "Bright Solutions",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ010",
                    Amount = 12000,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Stale,
                    CounterpartyName = "Classic Traders",
                    CounterpartyType = "Customer",
                    OurBankAccount = "ICICI - 4455",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-120),
                    StaleOn = DateTime.Today.AddDays(-30),
                    ReceivedOn = DateTime.Today.AddDays(-100),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-100),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ011",
                    Amount = 38500,
                    Direction = ChequeDirection.Outgoing,
                    Status = ChequeStatus.Issued,
                    CounterpartyName = "Anand Logistics",
                    CounterpartyType = "Vendor",
                    OurBankAccount = "HDFC - 8899",
                    BranchId = blrBranch,
                    Branch = "BLR - SofaCraft Experience Store - Bengaluru",
                    ChequeDate = DateTime.Today.AddDays(-7),
                    IssuedOn = DateTime.Today.AddDays(-7),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-7),
                    PreparedOn = DateTime.Today.AddDays(-8),
                    PrintedOn = DateTime.Today.AddDays(-8),
                    SourceModule = "AP",
                    SourceDocumentType = "VendorPayment",
                    PayeeName = "Anand Logistics",
                    Currency = "INR"
                },

                new ChequeModel
                {
                    ChequeNumber = "CHQ012",
                    Amount = 67000,
                    Direction = ChequeDirection.Incoming,
                    Status = ChequeStatus.Deposited,
                    CounterpartyName = "Sunrise Furniture Co",
                    CounterpartyType = "Customer",
                    OurBankAccount = "SBI - 9911",
                    BranchId = hqBranch,
                    Branch = "HO - SofaCraft Head Office & Factory - Chennai",
                    ChequeDate = DateTime.Today.AddDays(-8),
                    DepositedOn = DateTime.Today.AddDays(-6),
                    ReceivedOn = DateTime.Today.AddDays(-7),
                    IssuedOrReceivedOn = DateTime.Today.AddDays(-7),
                    PreparedOn = DateTime.Today.AddDays(-8),
                    SourceModule = "AR",
                    SourceDocumentType = "CustomerPayment",
                    PayeeName = "SofaCraft Furnishings",
                    DrawerBankName = "Kotak Mahindra Bank",
                    DrawerBankBranch = "T. Nagar",
                    Currency = "INR"
                }
            };
        }
    }
}
