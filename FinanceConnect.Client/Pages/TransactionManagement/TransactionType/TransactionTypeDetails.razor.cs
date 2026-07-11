using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionType
{
    public partial class TransactionTypeDetails
    {
        [Parameter] public Guid TypeId { get; set; }

        TransactionTypeModel? SelectedType;

        string? PostingProfileName;
        string? DocumentSeriesName;

        protected override void OnInitialized()
        {
            SelectedType = TypeService.GetById(TypeId);

            if (SelectedType == null)
                return;

            if (SelectedType.DefaultPostingProfileId.HasValue)
            {
                var profile = PostingProfileService.GetById(
                    SelectedType.DefaultPostingProfileId.Value);

                PostingProfileName = profile?.ProfileName;
            }

            if (SelectedType.DocumentNumberSeriesId.HasValue)
            {
                var series = DocSeriesService.GetById(
                    SelectedType.DocumentNumberSeriesId.Value);

                DocumentSeriesName = series?.SeriesName;
            }
        }

        string GetCompanyName(Guid? id)
        {
            return TypeService
                .GetCompanies()
                .FirstOrDefault(c => c.Id == id)
                ?.LegalName ?? "-";
        }

        void GoBack()
        {
            Nav.NavigateTo("/transaction-types");
        }
    }
}
