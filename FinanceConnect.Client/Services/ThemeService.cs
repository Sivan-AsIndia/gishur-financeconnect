using FinanceConnect.Client.ViewModels;
using Microsoft.JSInterop;

public class ThemeService
{
    public ThemeSettingsViewModel CurrentSettings { get; set; } = new();

    private readonly IJSRuntime _js;

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task ApplyThemeAsync()
    {
        await _js.InvokeVoidAsync("themeBuilder.applyTheme", CurrentSettings);
    }
}
