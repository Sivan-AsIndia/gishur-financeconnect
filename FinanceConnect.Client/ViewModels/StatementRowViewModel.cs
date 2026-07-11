using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Client.Models
{
    public class AccountStatementRow
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public string AccountNature { get; set; }
        public string Category { get; set; }

        public int DisplayOrder { get; set; }
        public string ChartOfAccounts { get; set; } = "COA - INDIA";
        public string AccountGroup { get; set; } = "Asset";
        public string Nature { get; set; } = "Asset";
        public string StatementType { get; set; } = "Balance Sheet";
        public string NormalBalance { get; set; } = "Debit";

        public bool IsPostable { get; set; } = true;
        public bool IsControl { get; set; }
        public bool BranchMandatory { get; set; } = true;
        public bool AllowManual { get; set; } = true;
        public bool IsCash { get; set; }
        public bool IsBank { get; set; }

        public string Status { get; set; } = "Draft";
        public string LockReason { get; set; }
    }

}
