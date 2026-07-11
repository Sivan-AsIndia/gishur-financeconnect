using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankStatement
{
    public partial class ImportBankStatement
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        [Inject] BankStatementService StatementService { get; set; } = default!;
        [Inject] BankAccountService BankAccountService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        public CompanyModel Company = new();

        private EditContext _editContext;
        bool isInitialized = false;
        int CurrentStep = 1;

        List<PreviewLine> PreviewLines = new();
        bool ShowPreview = false;
        Guid selectedBankAccount ;
        byte[]? UploadedFileBuffer;
        string? UploadedFileName;
        private IBrowserFile? UploadedFile;
        BankStatementModel Statement = new();
        List<BankAccountModel> BankAccounts = new();
        private HashSet<int> validationAttemptedSteps = new();
        private string? SelectedBankAccountCode;
        private string? SelectedBankAccountName;

        List<string> Profiles = new()
        {
            "HDFC_CSV_V1",
            "SBI_CSV_V1",
            "ICICI_XLSX_V1",
            "Generic_CSV",
            "OFX",
            "MT940"
        };

        List<WizardStep> Steps = new()
        {
            new("Identity", "Bank Account & Scope", "ti ti-building-bank"),
            new("File Metadata", "Statement File", "ti ti-file-upload"),
            new("Parsing Rules", "Profile & Mapping", "ti ti-settings"),
        };

        Guid SelectedBankAccount
        {
            get => selectedBankAccount;
            set => selectedBankAccount = value;
        }
        protected override void OnInitialized()
        {
            BankAccounts = BankAccountService.GetAll();

            Statement = new BankStatementModel
            {
            };

            _editContext = new EditContext(Statement);

            isInitialized = true;
        }

        private void ChangeBankAccount(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id) && id != Guid.Empty)
            {
                selectedBankAccount = id;
            }
            else
            {
                selectedBankAccount = Guid.Empty;
                SelectedBankAccountCode = null;
                SelectedBankAccountName = null;
                return;
            }

            var acc = BankAccounts
                .FirstOrDefault(a => a.Id == selectedBankAccount);

            if (acc != null)
            {
                Statement.BankAccountId = acc.Id;

                SelectedBankAccountCode = acc.BankAccountCode;
                SelectedBankAccountName = acc.BankAccountName;

                Statement.CompanyId = acc.CompanyId;
                Statement.BranchId = acc.BranchId;
                Statement.BranchName = acc.BranchName;
                Statement.BankAccountName = SelectedBankAccountName;

                Company = MasterDataService.GetCompanyById(acc.CompanyId);

                if (Company != null)
                    Statement.CompanyName = Company.LegalName;
            }
            else
            {
                ToastService.ShowError("Selected Bank Account not found");
            }
        }


        private bool IsCurrentStepValid(int step)
        {
            return step switch
            {
                1 => !string.IsNullOrWhiteSpace(Statement.CompanyName)
         && !string.IsNullOrWhiteSpace(Statement.BranchName)
      && Statement.BankAccountId != Guid.Empty,

                _ => true
            };
        }

        async Task HandleFile(InputFileChangeEventArgs e)
        {
            UploadedFile = e.File;

            if (UploadedFile == null)
                return;

            // Allowed file types
            var allowedTypes = new HashSet<string>
                {
                    "CSV",
                    "XLSX",
                    "OFX",
                    "MT940",
                    "TXT"
                };

            var extension = Path.GetExtension(UploadedFile.Name)
                .Replace(".", "")
                .ToUpperInvariant();

            if (!allowedTypes.Contains(extension))
            {
                UploadedFile = null;
                UploadedFileBuffer = null;
                UploadedFileName = null;

                ToastService.ShowError(
                    $"Unsupported file type: .{extension}. Allowed types: CSV, XLSX, OFX, MT940, TXT"
                );
                return;
            }

            // Accept file
            UploadedFileName = UploadedFile.Name; 
            Statement.FileNameOriginal = UploadedFile.Name;
            Statement.FileSizeBytes = UploadedFile.Size;
            Statement.FileType = extension;

            // Copy file into memory NOW (before DOM changes)
            using var stream = UploadedFile.OpenReadStream(100 * 1024 * 1024);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            UploadedFileBuffer = ms.ToArray();

            SeedPreviewLines(); // optional preview for Demo purpose
        }


        private bool ShowFieldError(int step, string field)
        {
            if (!validationAttemptedSteps.Contains(step))
                return false;

            return field switch
            {
                "BankAccountId" => Statement.BankAccountId==Guid.Empty,
                "FileNameOriginal" => string.IsNullOrWhiteSpace(Statement.FileNameOriginal),
                "StatementProfile" => Statement.StatementProfile == null,
                "ProfileVersion" => string.IsNullOrWhiteSpace(Statement.ProfileVersion),

                _ => false
            };
        }
        async Task Next()
        {
            validationAttemptedSteps.Add(CurrentStep);

            if (!IsCurrentStepValid())
            {
                StateHasChanged();
                return;
            }

            if (CurrentStep < Steps.Count)
                CurrentStep++;

            await JS.InvokeVoidAsync("scrollToStep", CurrentStep - 1);
        }

        void Back()
        {
            if (CurrentStep > 1)
                CurrentStep--;
        }

        bool IsCurrentStepValid()
        {
            return CurrentStep switch
            {
                1 => Statement.BankAccountId != Guid.Empty,

                2 => UploadedFile != null,

                3 => Statement.StatementProfile != null
                  && !string.IsNullOrWhiteSpace(Statement.ProfileVersion),

                _ => true
            };
        }


        async Task Save()
        {
            // Validate all steps with inline errors
            for (int i = 1; i <= Steps.Count; i++)
            {
                validationAttemptedSteps.Add(i);
            }

            if (UploadedFileBuffer == null || string.IsNullOrWhiteSpace(UploadedFileName))
            {
                CurrentStep = 2;
                StateHasChanged();
                return;
            }

            if (Statement.StatementProfile == null || string.IsNullOrWhiteSpace(Statement.ProfileVersion))
            {
                CurrentStep = 3;
                StateHasChanged();
                return;
            }

            try
            {
                await StatementService.Import(
                    Statement,
                    UploadedFileBuffer,
                    UploadedFileName);

                ToastService.ShowSuccess("Bank statement imported successfully");
                Nav.NavigateTo("/bank-statements");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }


        string IndicatorToppx => $"{40 + ((CurrentStep - 1) * 76)}px";

        string StepClass(int i)
        {
            if (i < CurrentStep) return "done";
            if (i == CurrentStep) return "active";
            return "";
        }

        public record WizardStep(string Title, string Description, string Icon);


        void SeedPreviewLines()
        {
            PreviewLines.Clear();

            decimal runningBalance = 125000.00m;

            List<(string Date, string Narration, decimal Debit, decimal Credit)> seedData = new()
                {
                    ("01 Jan 2026", "OPENING BALANCE", 0m, 0m),
                    ("02 Jan 2026", "UPI CREDIT - ABC STORES", 0m, 2500.00m),
                    ("03 Jan 2026", "NEFT DEBIT - OFFICE RENT", 15000.00m, 0m),
                    ("04 Jan 2026", "UPI CREDIT - CLIENT PAYMENT", 0m, 45000.00m),
                    ("05 Jan 2026", "ATM CASH WITHDRAWAL", 10000.00m, 0m),
                    ("06 Jan 2026", "SMS CHARGE", 25.00m, 0m),
                    ("07 Jan 2026", "INTEREST CREDIT", 0m, 125.00m),
                    ("08 Jan 2026", "NEFT DEBIT - SUPPLIER", 32000.00m, 0m),
                    ("09 Jan 2026", "UPI CREDIT - ONLINE SALE", 0m, 8750.00m),
                    ("10 Jan 2026", "CLOSING BALANCE", 0m, 0m)
                };

            foreach (var row in seedData)
            {
                runningBalance += row.Credit;
                runningBalance -= row.Debit;

                PreviewLines.Add(new PreviewLine
                {
                    Date = row.Date,
                    Narration = row.Narration,
                    Debit = row.Debit == 0 ? "-" : row.Debit.ToString("N2"),
                    Credit = row.Credit == 0 ? "-" : row.Credit.ToString("N2"),
                    Balance = runningBalance.ToString("N2")
                });
            }

            ShowPreview = true;
        }


    }
}
