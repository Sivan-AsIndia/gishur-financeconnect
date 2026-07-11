using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionLine
{
    public partial class TransactionLines
    {
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Parameter] public Guid TransactionId { get; set; }
        private EditContext _editContext;
        FinancialTransactionModel Tx = new();
        List<TransactionLineModel> UiLines = new();
        private Dictionary<Guid, HashSet<string>> _lineErrors = new();
        bool isInitialized;

        decimal TotalAmount;

        bool IsReadOnly => Tx.Status != "Draft";

        string[] LineTypes = new[]
        {
        "PRINCIPAL","TAX","DISCOUNT","CHARGE",
        "ROUNDING","FX_ADJUSTMENT","WITHHOLDING","OTHER"
    };

        string[] PostingCategories = new[]
        {
        "EXPENSE","REVENUE","BANK","AR_CONTROL","AP_CONTROL",
        "TAX_INPUT","TAX_OUTPUT","ROUNDING_GAIN_LOSS",
        "DISCOUNT_ALLOWED","DISCOUNT_RECEIVED","FX_GAIN_LOSS","CLEARING"
    };

        protected override void OnInitialized()
        {
            Tx = TxService.GetById(TransactionId);

            if (Tx != null)
            {
                UiLines = LineService.GetByTransaction(TransactionId);
                if (UiLines != null && Tx.CreatedBy == "seed")
                {
                    TriggerSeedIfRequired();
                }
                RecalculateTotals();
                _editContext = new EditContext(this);
                isInitialized = true;
            }
            else
            {
                ToastService.ShowError("Transaction not found.");
            }

        }

        void TriggerSeedIfRequired()
        {
            bool shouldSeed =
                Tx.CreatedBy == "seed" &&
                !UiLines.Any();

            if (!shouldSeed)
                return;

            // Example seed: PRINCIPAL + TAX
            var total = Tx.TransactionAmount;
            const decimal taxRate = 0.18m;

            // Reverse calculate principal so total matches exactly
            var principalAmount = Math.Round(total / (1 + taxRate), 2);
            var taxAmount = Math.Round(total - principalAmount, 2);

            // Safety fix for rounding drift
            if (principalAmount + taxAmount != total)
            {
                taxAmount = total - principalAmount;
            }

            // PRINCIPAL LINE
            LineService.SeedLine(new TransactionLineModel
            {
                TransactionLineId = Guid.NewGuid(),
                FinancialTransactionId = Tx.FinancialTransactionId,

                LineNumber = 10,
                LineType = TransactionLineType.PRINCIPAL,
                PostingCategory = PostingCategory.EXPENSE,
                LineNarration = "Seeded principal line",

                Quantity = 1,
                UnitRate = principalAmount,
                LineAmount = principalAmount,

                BranchId = Tx.BranchId,
                IsTaxLine = false,
                IsSystemGenerated = true,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });

            // TAX LINE
            LineService.SeedLine(new TransactionLineModel
            {
                TransactionLineId = Guid.NewGuid(),
                FinancialTransactionId = Tx.FinancialTransactionId,

                LineNumber = 20,
                LineType = TransactionLineType.TAX,
                PostingCategory = PostingCategory.TAX_INPUT,
                LineNarration = "Seeded tax line (18%)",

                Quantity = 1,
                UnitRate = taxAmount,
                LineAmount = taxAmount,

                BranchId = Tx.BranchId,
                IsTaxLine = true,
                IsSystemGenerated = true,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });


            // Reload after seeding
            UiLines = LineService.GetByTransaction(Tx.FinancialTransactionId)
                ?? new List<TransactionLineModel>();

            RecalculateTotals();
        }

        void AddLine()
        {
            var nextNo = UiLines.Any()
                ? UiLines.Max(x => x.LineNumber) + 10
                : 10;

            var line = new TransactionLineModel
            {
                FinancialTransactionId = TransactionId,
                LineNumber = nextNo,
                BranchId = Tx.BranchId,
                BaseAmount = 0
            };

            UiLines.Add(line);
        }

        void Delete(TransactionLineModel line)
        {
            if (line.TransactionLineId != Guid.Empty)
                LineService.Delete(line.TransactionLineId);

            UiLines.Remove(line);
            RecalculateTotals();
        }

        void OnAmountChanged(TransactionLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var value))
            {
                line.LineAmount = value;
                line.BaseAmount = value; // FX conversion applied at posting stage
                RecalculateTotals();
            }
        }

        void Cancel()
        {
            Nav.NavigateTo("/financial-transactions");
        }

        void OnQtyRateChanged(TransactionLineModel line)
        {
            if (line.Quantity <= 0)
                line.Quantity = 1;

            if (line.UnitRate < 0)
                line.UnitRate = 0;

            line.LineAmount = Math.Round(line.Quantity * line.UnitRate, 2);

            RecalculateTotals();
        }

        void OnAmountManualChanged(TransactionLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var val))
            {
                line.LineAmount = Math.Round(val, 2);

                // Reset qty/rate to indicate manual override
                line.Quantity = 1;
                line.UnitRate = val;
            }

            RecalculateTotals();
        }

        void RecalculateTotals()
        {
            TotalAmount = UiLines.Sum(l => l.LineAmount);
            InvokeAsync(StateHasChanged);
        }

        void HandleSubmit()
        {
            //ValidateLines();

            //if (_lineErrors.Any())
            //{
            //    ToastService.ShowError("Fix validation errors before saving lines.");
            //    StateHasChanged();
            //    return;
            //}



            SaveLines();
        }
        void SaveLines()
        {
            if (UiLines.Any(l => l.PostingCategory == null))
            {
                ToastService.ShowError("Posting Category Required.");
                return;
            }
            if (TotalAmount != Tx.TransactionAmount)
            {
                ToastService.ShowError("Lines total must match header amount.");
                return;
            }
            try
            {
                foreach (var line in UiLines)
                {
                    if (line.TransactionLineId == Guid.Empty)
                    {
                        LineService.Add(line);
                    }

                    else
                    {
                        if (!line.IsSystemGenerated)
                        {
                            LineService.Update(line);
                        }
                    }

                        
                }

                ToastService.ShowSuccess("Transaction lines saved successfully");
                Nav.NavigateTo("/financial-transactions");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        void ValidateLines()
        {
            _lineErrors.Clear();

            foreach (var line in UiLines)
            {
                var errors = new HashSet<string>();

                if (!line.LineType.HasValue)
                    errors.Add(nameof(line.LineType));

                if (!line.PostingCategory.HasValue)
                    errors.Add(nameof(line.PostingCategory));

                if (line.LineAmount <= 0)
                    errors.Add(nameof(line.LineAmount));

                if (errors.Any())
                    _lineErrors[line.TransactionLineId] = errors;
            }
        }

        bool HasError(TransactionLineModel line, string field)
        {
            return _lineErrors.TryGetValue(line.TransactionLineId, out var fields)
                   && fields.Contains(field);
        }

        string ErrorClass(TransactionLineModel line, string field)
        {
            return HasError(line, field) ? "is-invalid" : "";
        }

        string GetCompanyName(Guid? companyId)
        {
            if (companyId == null || companyId == Guid.Empty)
                return "-";

            var company = MasterDataService
                .GetAllCompanies()
                .FirstOrDefault(c => c.Id == companyId.Value);

            return company?.LegalName ?? "-";
        }

        string GetBranchName(Guid BranchId)
        {
            if (BranchId == null || BranchId == Guid.Empty)
                return "-";

            var Branch = BranchService.GetById(BranchId);

            return Branch?.BranchName ?? "-";
        }

        string GetCurrencyName(Guid? CurrencyId)
        {
            var CurrencyName = CurrencyId.HasValue ? MasterDataService
            .GetCurrencyById(CurrencyId.Value)?.CurrencyName ?? "—"
            : "—";
            return CurrencyName;
        }


        string GetStatusBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary-transparent text-secondary",
                "Submitted" => "bg-info-transparent text-info",
                "Approved" => "bg-warning-transparent text-warning",
                "Posted" => "bg-success-transparent text-success",
                "Reversed" => "bg-danger-transparent text-danger",
                _ => "bg-dark"
            };
        }
    }
}

