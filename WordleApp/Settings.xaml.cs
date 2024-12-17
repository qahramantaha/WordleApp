using CommunityToolkit.Maui.Views;

namespace WordleApp;

public partial class Settings : Popup
{
    public Settings()
    {
        InitializeComponent();
    }

    private void darkMode_switch_Toggled(object sender, ToggledEventArgs e)
    {

        Application.Current.Resources["IsDarkMode"] = e.Value;
        ToggleTheme(e.Value);


    }

    private void hardMode_switch_Toggled(object sender, ToggledEventArgs e)
    {

        Application.Current.Resources["IsHardMode"] = e.Value;


    }

    private void ToggleTheme(bool isDarkMode)
    {
        var app = (App)Application.Current;

        if (isDarkMode)
        {
            app.Resources["BackgroundColor"] = Colors.Black;
            app.Resources["TextColor"] = Colors.White;
        }
        else
        {
            app.Resources["BackgroundColor"] = Colors.White;
            app.Resources["TextColor"] = Colors.Black;
        }
    }
}
