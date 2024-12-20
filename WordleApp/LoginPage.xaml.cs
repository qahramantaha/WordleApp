
namespace WordleApp
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            string playerName = PlayerNameEntry.Text?.Trim(); //Capture the player's name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                await DisplayAlert("Error", "Please enter your name to proceed.", "OK");
                return;
            }

            // Save player's name
            Preferences.Set("PlayerName", playerName);

            // Navigate to WelcomePage
            await Navigation.PushAsync(new WelcomePage(playerName));
        }
    }
}