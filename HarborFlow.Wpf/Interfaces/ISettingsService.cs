
namespace HarborFlow.Wpf.Interfaces
{
    public interface ISettingsService
    {
        ThemeType GetTheme();
        void SetTheme(ThemeType theme);
    }
}
