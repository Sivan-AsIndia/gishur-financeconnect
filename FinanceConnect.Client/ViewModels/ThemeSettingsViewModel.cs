namespace FinanceConnect.Client.ViewModels
{
    public class ThemeSettingsViewModel
    {
        public string PrimaryColor { get; set; } = "#FE9F43";
        public string SecondaryColor { get; set; } = "#051a2c";
        public string IconColor { get; set; } = "#9ca3af";
        public string HeadingColor { get; set; } = "#FE9F43";
        public bool IsDarkMode { get; set; } = false;
        public string FontFamily { get; set; } = "Inter, sans-serif";
        public string FontSize { get; set; } = "16px";
        public int ShadowStrength { get; set; } = 15;
        public int BorderRadius { get; set; } = 8;
        public int ButtonRadius { get; set; } = 8;
        public string LayoutMode { get; set; } = "box";
        public string? MenuFont { get; set; }
        public string? HeadingFont { get; set; }
        public string? NumberFont { get; set; }
        public string? OtherFont { get; set; }
        public string SelectedColorId { get; set; } = "blue1";
        public string SelectedSecondaryId { get; set; } = "blue2";
        public string SelectedIconId { get; set; } = "blue12";
        public string SelectedHeadingId { get; set; } = "blue13";
    }
}
