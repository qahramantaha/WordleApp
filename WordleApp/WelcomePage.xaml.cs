using CommunityToolkit.Maui.Views;

namespace WordleApp;

public partial class WelcomePage : ContentPage
{
    private string _playerName;
    private string _playerFilePath;

    public WelcomePage(string playerName)
    {
        InitializeComponent();
        _playerName = playerName;
        _playerFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{_playerName}_stats.txt");
        DisplayWelcomeMessage();
        LoadPlayerProfile();

    }

    private void LoadPlayerProfile()
    {
        if (File.Exists(_playerFilePath))
        {
            // Load stats from the file
            var lines = File.ReadAllLines(_playerFilePath);
            // Example: Stats are stored as lines (Wins, Streak, Games Played)
            int numWins = int.Parse(lines[0]);
            int streak = int.Parse(lines[1]);
            int gamesPlayed = int.Parse(lines[2]);

            // Update the welcome page or app with the player's stats
            GameTagline.Text = $"Games Played: {gamesPlayed}, Wins: {numWins}, Streak: {streak}";
        }
        else
        {
            // Create a new file for the player
            File.WriteAllLines(_playerFilePath, new[] { "0", "0", "0" }); // Default stats: 0 Wins, 0 Streak, 0 Games Played
            GameTagline.Text = "Welcome to your first game of Wordle!";
        }
    }
    private void DisplayWelcomeMessage()
    {
        GameTitle.Text = $"Welcome, {_playerName}!";
    }

    private async void PlayButton_Clicked(object sender, EventArgs e)
    {
        // Navigate to MainPage and pass the player's name
        await Navigation.PushAsync(new MainPage(_playerName));
    }
    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
        //redirect to settings page upon clicking
        await this.ShowPopupAsync(new Settings());
    }

    private async void HowToPlay_Clicked(object sender, EventArgs e)
    {
        //redirect to HowToPlay page upon clicking
        await this.ShowPopupAsync(new HowToPlay());
    }
}