namespace FinanceConnect.Client.ViewModels
{
    #region Static Enums and Lookup Classes

    public static class JurisdictionTypes
    {
        public static readonly string[] All = new[] { "State", "Union Territory", "Province", "Territory", "Region", "Other" };
    }

    public static class MasterDataStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }

    public static class Regions
    {
        public static readonly string[] All = new[] { "Asia", "Europe", "North America", "South America", "Africa", "Oceania", "Middle East" };
    }

    public static class CurrencyTypes
    {
        public const string Fiat = "Fiat";
        public const string Crypto = "Crypto";
        public const string Other = "Other";
        public static readonly string[] All = new[] { Fiat, Crypto, Other };
    }

    public static class SymbolPositions
    {
        public const string Prefix = "Prefix";
        public const string Suffix = "Suffix";
        public static readonly string[] All = new[] { Prefix, Suffix };
    }

    public static class RoundingModes
    {
        public const string RoundHalfUp = "Round Half Up";
        public const string RoundHalfDown = "Round Half Down";
        public const string BankersRounding = "Bankers Rounding";
        public static readonly string[] All = new[] { RoundHalfUp, RoundHalfDown, BankersRounding };
    }

    public static class LegalStructures
    {
        public const string PrivateLimited = "Private Limited";
        public const string PublicLimited = "Public Limited";
        public const string LLP = "LLP";
        public const string Partnership = "Partnership";
        public const string SoleProprietorship = "Sole Proprietorship";
        public const string TrustSociety = "Trust/Society";
        public const string GovernmentPSU = "Government/PSU";
        public const string Other = "Other";
        public static readonly string[] All = new[] { PrivateLimited, PublicLimited, LLP, Partnership, SoleProprietorship, TrustSociety, GovernmentPSU, Other };
    }

    public static class CompanyStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }

    public static class FiscalYearMonths
    {
        public static readonly (int Value, string Name)[] All = new[]
        {
            (1, "January"), (2, "February"), (3, "March"), (4, "April"),
            (5, "May"), (6, "June"), (7, "July"), (8, "August"),
            (9, "September"), (10, "October"), (11, "November"), (12, "December")
        };
    }

    public static class RateTypes
    {
        public const string Spot = "Spot";
        public const string MonthAverage = "MonthAverage";
        public const string Customs = "Customs";
        public const string BankRate = "BankRate";
        public const string Manual = "Manual";
        public static readonly string[] All = new[] { Spot, MonthAverage, Customs, BankRate, Manual };

        public static string GetDisplayName(string rateType) => rateType switch
        {
            Spot => "Spot (Daily)", MonthAverage => "Month Average", Customs => "Customs",
            BankRate => "Bank Rate", Manual => "Manual", _ => rateType
        };
    }

    public static class SourceTypes
    {
        public const string ManualEntry = "ManualEntry";
        public const string CSVImport = "CSVImport";
        public const string BankStatement = "BankStatement";
        public const string GovernmentPublished = "GovernmentPublished";
        public const string LiveAPI = "LiveAPI";
        public const string Other = "Other";
        public static readonly string[] All = new[] { ManualEntry, CSVImport, BankStatement, GovernmentPublished, LiveAPI, Other };

        public static string GetDisplayName(string sourceType) => sourceType switch
        {
            ManualEntry => "Manual Entry", CSVImport => "CSV Import", BankStatement => "Bank Statement",
            GovernmentPublished => "Government Published", LiveAPI => "Live API", Other => "Other", _ => sourceType
        };
    }

    public static class ExchangeRateStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }
    public static class LockStatus
    {
        public const string Unlocked = "Unlocked";
        public const string LockedAfterPosting = "LockedAfterPosting";
        public const string LockedByController = "LockedByController";
        public static readonly string[] All = new[] { Unlocked, LockedAfterPosting, LockedByController };
    }

    public static class GLSourceTypes
    {
        public const string JournalEntry = "JournalEntry";
        public const string OpeningBalance = "OpeningBalance";
        public const string VendorBill = "VendorBill";
        public const string VendorPayment = "VendorPayment";
        public const string CustomerInvoice = "CustomerInvoice";
        public const string CustomerReceipt = "CustomerReceipt";
        public const string BankTransaction = "BankTransaction";
        public const string AssetTransaction = "AssetTransaction";
        public const string SystemAdjustment = "SystemAdjustment";
        public const string Other = "Other";
        public static readonly string[] All = new[] { JournalEntry, OpeningBalance, VendorBill, VendorPayment, CustomerInvoice, CustomerReceipt, BankTransaction, AssetTransaction, SystemAdjustment, Other };

        public static string GetDisplayName(string type) => type switch
        {
            JournalEntry => "Journal Entry", OpeningBalance => "Opening Balance", VendorBill => "Vendor Bill",
            VendorPayment => "Vendor Payment", CustomerInvoice => "Customer Invoice", CustomerReceipt => "Customer Receipt",
            BankTransaction => "Bank Transaction", AssetTransaction => "Asset Transaction",
            SystemAdjustment => "System Adjustment", Other => "Other", _ => type
        };
    }

    public static class CloseStatusTypes
    {
        public const string Calculated = "Calculated";
        public const string Verified = "Verified";
        public const string Locked = "Locked";
        public const string Reversed = "Reversed";
        public static readonly string[] All = new[] { Calculated, Verified, Locked, Reversed };
    }

    #endregion
}
