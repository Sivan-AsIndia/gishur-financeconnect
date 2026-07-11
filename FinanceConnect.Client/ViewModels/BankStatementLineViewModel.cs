using System;

namespace FinanceConnect.Client.ViewModels
{
    public enum StatementLineDirectionType
    {
        Debit = 1,
        Credit = 2
    }
    public enum ParseStatusType
    {
        Parsed = 1,
        ParseWarning = 2,
        ParseError = 3
    }

    public enum ReconciliationStatusType
    {
        Unmatched = 1,
        Suggested = 2,
        Matched = 3,
        FinalizedLocked = 4,
        Excluded = 5
    }
    public class BankStatementLineModel
    {

        public Guid BankStatementLineId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid BankStatementId { get; set; }
        public Guid BankAccountId { get; set; }
        public int LineNumber { get; set; }


        public DateTime TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime ImportBatchDate { get; set; }



        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public StatementLineDirectionType Direction { get; set; }


        public decimal Amount { get; set; }

        public Guid CurrencyId { get; set; }
        public decimal? RunningBalance { get; set; }     // Optional


        public string? BankProvidedTransactionId { get; set; }
        public string? ReferenceText { get; set; }
        public string? NarrationRaw { get; set; } 
        public string? NarrationNormalized { get; set; }

        public string? UTRNumberExtracted { get; set; }      // Derived (NEFT/RTGS/IMPS/UPI)
        public string? ChequeNumberExtracted { get; set; }
        public string? CounterpartyNameExtracted { get; set; }
        public string? TransactionCode { get; set; }


        public string LineHashSHA256 { get; set; } = string.Empty;

        public bool IsDuplicateInFile { get; set; }
        public Guid? DuplicateGroupId { get; set; }
        public bool? IsDuplicateAcrossStatements { get; set; }

        public ParseStatusType ParseStatus { get; set; }
        public string? ParseWarningMessage { get; set; }


        public string? RawLineJson { get; set; }



        public ReconciliationStatusType ReconciliationStatus { get; set; }

        public Guid? SuggestedBankTransactionId { get; set; }
        public decimal? MatchConfidenceScore { get; set; }

        public Guid? MatchedBankTransactionId { get; set; }
        public decimal? MatchedAmount { get; set; }

        public Guid? BankReconciliationId { get; set; }

        public DateTime? MatchedOn { get; set; }
        public string? MatchedBy { get; set; }

        public string? ExcludeReason { get; set; }


        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
