using CommunityToolkit.Maui.Views;

namespace WordleApp;

public partial class Settings : Popup
{
    public Settings()
    {
        InitializeComponent();
    }

    // Updates the "IsDarkMode" resource and applies the selected theme
    private void darkMode_switch_Toggled(object sender, ToggledEventArgs e)
    {

        Application.Current.Resources["IsDarkMode"] = e.Value;
        ToggleTheme(e.Value); 


    }

    // Updates the "IsHardMode" resource in the application settings
    private void hardMode_switch_Toggled(object sender, ToggledEventArgs e)
    {

        Application.Current.Resources["IsHardMode"] = e.Value;


    }

    // Applies the appropriate theme (dark or light) by updating app resource colors
    private void ToggleTheme(bool isDarkMode)
    {
        var app = (App)Application.Current;

        if (isDarkMode)
        {
            app.Resources["BackGroundColor"] = Colors.Black;
            app.Resources["TextColor"] = Colors.White;
        }
        else
        {
            app.Resources["BackGroundColor"] = Colors.White;
            app.Resources["TextColor"] = Colors.Black;
        }
    }
}
