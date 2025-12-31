using MudBlazor;

namespace FinancialManager.Web.Theme;

public static class AppTheme
{
    public static MudTheme DefaultTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#6366F1",        // Lighter Indigo for dark mode
            Secondary = "#22C55E",
            Tertiary = "#F97316",
            Success = "#22C55E",
            Info = "#3B82F6",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Dark = "#0F172A",
            Background = "#0F172A",      // Dark slate
            Surface = "#1E293B",         // Lighter dark slate
            TextPrimary = "#E5E7EB",
            TextSecondary = "#94A3B8",
            AppbarBackground = "#1E293B",
            DrawerBackground = "#0F172A",
            DrawerText = "#E5E7EB",
            ActionDefault = "#94A3B8"
        },
        
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px",
            AppbarHeight = "64px"
        }
    };
}
