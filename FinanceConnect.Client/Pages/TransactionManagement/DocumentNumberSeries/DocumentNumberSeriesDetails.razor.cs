using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.TransactionManagement.DocumentNumberSeries
{
    public partial class DocumentNumberSeriesDetails
    {

        [Parameter] public Guid SeriesId { get; set; }

        DocumentNumberSeriesModel? SelectedSeries;

        protected override void OnInitialized()
        {
            SelectedSeries = SeriesService.GetById(SeriesId);
        }

        string GetCompanyName(Guid? companyId)
        {
            if (!companyId.HasValue) return "-";

            return MasterDataService
                .GetAllCompanies()
                .FirstOrDefault(c => c.Id == companyId.Value)
                ?.LegalName ?? "-";
        }

        void GoBack()
        {
            Nav.NavigateTo("/document-series");
        }
    }
}
