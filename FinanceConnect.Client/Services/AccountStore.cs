using FinanceApp.Client.Models;
using FinanceConnect.Client.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AccountStore
{
    public List<AccountStatementRow> Accounts { get; set; }

    public Task<AccountStatementRow> GetAccountByCode(string code)
    {
        var account = Accounts.FirstOrDefault(a => a.AccountCode == code);
        return Task.FromResult(account ?? new AccountStatementRow());
    }

    public AccountStore()
    {
        // Default accounts (first time load)
        Accounts = new List<AccountStatementRow>
        {
            new AccountStatementRow { AccountCode="1000", AccountName="Cash on Hand", Description="Petty cash for SofaCraft", AccountNature="Asset", Category="Current Asset", Status="Active" },
            new AccountStatementRow { AccountCode="1010", AccountName="Bank - HDFC Current A/c", Description="Primary operating bank account", AccountNature="Asset", Category="Current Asset", Status="Active" },
            new AccountStatementRow { AccountCode="1100", AccountName="Accounts Receivable", Description="Customer receivables - Sofa sales", AccountNature="Asset", Category="Current Asset", Status="Active" },
            new AccountStatementRow { AccountCode="1200", AccountName="Inventory - Raw Materials", Description="Fabric, foam, wood, hardware", AccountNature="Asset", Category="Current Asset", Status="Active" },
            new AccountStatementRow { AccountCode="1210", AccountName="Inventory - Finished Sofas", Description="Finished goods inventory", AccountNature="Asset", Category="Current Asset", Status="Active" },
            new AccountStatementRow { AccountCode="2000", AccountName="Accounts Payable", Description="Supplier payables", AccountNature="Liability", Category="Current Liability", Status="Active" },
            new AccountStatementRow { AccountCode="2100", AccountName="GST Payable", Description="Output GST minus input GST", AccountNature="Liability", Category="Tax Liability", Status="Active" },
            new AccountStatementRow { AccountCode="3000", AccountName="Owner's Equity", Description="Capital introduced", AccountNature="Equity", Category="Equity", Status="Active" },
            new AccountStatementRow { AccountCode="4000", AccountName="Sales - Sofas", Description="Revenue from sofa sales", AccountNature="Income", Category="Revenue", Status="Active" },
            new AccountStatementRow { AccountCode="4010", AccountName="Sales - Delivery Charges", Description="Delivery & installation income", AccountNature="Income", Category="Revenue", Status="Active" },
            new AccountStatementRow { AccountCode="5000", AccountName="COGS - Materials", Description="Fabric/Foam/Wood used in production", AccountNature="Expense", Category="Direct Expense", Status="Active" },
            new AccountStatementRow { AccountCode="5010", AccountName="COGS - Direct Labor", Description="Carpentry, upholstery, stitching labor", AccountNature="Expense", Category="Direct Expense", Status="Active" },
            new AccountStatementRow { AccountCode="6100", AccountName="Factory Utilities", Description="Electricity/water for factory", AccountNature="Expense", Category="Overhead", Status="Active" },
            new AccountStatementRow { AccountCode="6200", AccountName="Rent - Showroom", Description="Showroom rent", AccountNature="Expense", Category="Operating Expense", Status="Active" },
            new AccountStatementRow { AccountCode="6300", AccountName="Salaries & Wages", Description="Admin + sales salaries", AccountNature="Expense", Category="Operating Expense", Status="Active" },
            new AccountStatementRow { AccountCode="6400", AccountName="Marketing & Ads", Description="Digital/print marketing", AccountNature="Expense", Category="Operating Expense", Status="Active" },
            new AccountStatementRow { AccountCode="6500", AccountName="Bank Charges", Description="Bank fees and charges", AccountNature="Expense", Category="Operating Expense", Status="Active" },
        };
    }
}
